//
// WalletService.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Xamarin.Forms;

using NBitcoin;

using Liviano;
using Liviano.Bips;
using Liviano.Interfaces;
using Liviano.Utilities;
using Liviano.Extensions;
using Liviano.Models;
using Liviano.Exceptions;
using Liviano.Electrum;

using static Liviano.Electrum.ElectrumClient;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;

[assembly: Dependency(typeof(WalletService))]
namespace HodlWallet2.Core.Services
{
    public sealed class WalletService : IWalletService
    {
        public const int DEFAULT_NODES_TO_CONNECT = 4;

        public const string DEFAULT_NETWORK = "testnet";

        public static string USER_AGENT { get; } = $"{Liviano.Version.UserAgent}/hodlwallet:2.0/";

        object _Lock = new object();

        Network _Network;

        string _WalletId;

        public Serilog.ILogger Logger { set; get; }

        public BlockLocator ScanLocation { get; set; }

        public event EventHandler OnConfigured;
        public event EventHandler OnStarted;

        public bool IsStarted { get; private set; }
        public bool IsConfigured { get; private set; }

        public IWallet Wallet { get; private set; }

        /// <summary>
        /// Empty constructor that MvvmCross needs to start as a service
        /// </summary>
        public WalletService() { }

        async Task PeriodicSave()
        {
            while (true)
            {
                await Save();

                await Task.Delay(60_000);
            }
        }

        async Task Save()
        {
            await Task.Factory.StartNew(() =>
            {
                lock (_Lock)
                {
                    Wallet.Start();
                    Wallet.Storage.Save();
                }
            });
        }

        public void InitializeWallet(bool isLegacy = false)
        {
            string guid;
            if (SecureStorageService.HasWalletId())
            {
                guid = SecureStorageService.GetWalletId();
            }
            else
            {
                // Default uses the Guid class from System in .NET
                guid = Guid.NewGuid().ToString();

                SecureStorageService.SetWalletId(guid);
            }

            string network;
            if (SecureStorageService.HasNetwork())
            {
                network = SecureStorageService.GetNetwork();
            }
            else
            {
                network = DEFAULT_NETWORK;
                SecureStorageService.SetNetwork(network);
            }

            _Network = Hd.GetNetwork(network ?? DEFAULT_NETWORK);
            _WalletId = guid ?? Guid.NewGuid().ToString();

            ElectrumClient.OverwriteRecentlyConnectedServers(_Network);

            if (!SecureStorageService.HasMnemonic() || _WalletId == null)
            {
                Logger.Information("Wallet has been configured but not started yet due to the lack of mnemonic in the system");
                return;
            }

            if (isLegacy == true && !SecureStorageService.HasSeedBirthday())
            {
                Logger.Information("Legacy wallet has been configured but not started yet due to the lack of seed birthday in the system");
                return;
            }

            StartWalletWithWalletId();

            Logger.Information("Since wallet has a mnemonic, then start the wallet.");

            OnConfigured?.Invoke(this, null);
            IsConfigured = true;

            Logger.Information("Configured wallet.");
        }

        public void StartWalletWithWalletId()
        {
            Guard.NotNull(_WalletId, nameof(_WalletId));

            string mnemonic = SecureStorageService.GetMnemonic();
            string password = ""; // TODO password cannot be null. but it should be
                                  // change liviano load wallet to accept null passwords
                                  // But, since HODLWallet 1 didn't have passwords this is okay

#if DEBUG
            _Network = Hd.GetNetwork(DEFAULT_NETWORK);
#else
            _Network = Hd.GetNetwork();
#endif

            var storage = new WalletStorageProvider(_WalletId, _Network);

            if (storage.Exists())
            {
                Logger.Information("Loading a wallet because it exists");

                try
                {
                    Wallet = storage.Load();
                    Wallet.CurrentAssembly = IntrospectionExtensions.GetTypeInfo(typeof(WalletService)).Assembly;

                    Start();
                    Logger.Information("Wallet started.");

                    return;
                }
                catch (Exception ex)
                {
                    Logger.Information(ex.Message);

                    // What would lead to this?

                    // TODO: Defensive programming is a bad practice, this is a bad practice
                    if (!Hd.IsMnemonicOfWallet(new Mnemonic(mnemonic), (Wallet)Wallet, _Network))
                    {
                        lock (_Lock)
                        {
                            storage.Delete();
                        }
                    }
                }
            }

            Logger.Debug("Creating wallet ({guid}) with password: {password}", _WalletId, password);

            DateTimeOffset createdAt = SecureStorageService.HasSeedBirthday()
                ? DateTimeOffset.FromUnixTimeSeconds(SecureStorageService.GetSeedBirthday())
                : new DateTimeOffset(DateTime.UtcNow);

            Wallet = new Wallet { Id = _WalletId };

            Assembly assembly = IntrospectionExtensions.GetTypeInfo(typeof(WalletService)).Assembly;

            Wallet.Init(mnemonic, password, null, _Network, createdAt, storage, assembly);

            Wallet.AddAccount("bip141");

            if (!Wallet.Accounts.Any())
            {
                throw new WalletException("Account was unable to be initialized.");
            }

            var account = Wallet.Accounts[0].CastToAccountType();

            Logger.Debug($"Account Type: {account.GetType()}");
            Logger.Debug($"Added account with path: {account.HdPath}");

            Wallet.Storage.Save();

            // Add paper wallet clause

            Logger.Information("Wallet created.");

            // NOTE Do not delete this, this is correct, the wallet should start after it being configured.
            Start();

            Logger.Information("Wallet started.");
        }

        public void Start()
        {
            var start = new DateTimeOffset();
            var end = new DateTimeOffset();

            Wallet.SyncStarted += (obj, _) =>
            {
                start = DateTimeOffset.Now;

                Logger.Debug($"Syncing started at {start.LocalDateTime.ToLongTimeString()}");
            };

            Wallet.SyncFinished += (obj, _) =>
            {
                end = DateTimeOffset.UtcNow;

                Logger.Debug($"Syncing ended at {end.LocalDateTime.ToLongTimeString()}");
                Logger.Debug($"Syncing time: {(end - start).TotalSeconds}");
            };

            _ = PeriodicSave();

            _ = Wallet.Sync();

            OnStarted?.Invoke(this, null);
            IsStarted = true;
        }

        public static string GetNewMnemonic(string wordList = "english", int wordCount = 12)
        {
            return Hd.NewMnemonic(wordList, wordCount).ToString();
        }

        public bool IsAddressOwn(string address)
        {
            var parsedAddress = BitcoinAddress.Create(address, _Network);

            bool inInternal = Wallet.CurrentAccount.UsedInternalAddresses
                              .Contains(parsedAddress) ||
                              parsedAddress == Wallet.CurrentAccount.GetReceiveAddress();

            bool inExternal = Wallet.CurrentAccount.UsedExternalAddresses
                              .Contains(parsedAddress);

            return inInternal || inExternal;
        }

        /// <summary>
        /// Destroy wallet, deletes wallets file and disconnects from nodes
        /// </summary>
        /// <param name="dryRun">Do not delete anything just try</param>
        public void DestroyWallet(bool dryRun = false)
        {
            if (_Network == null)
            {
                string networkStr = SecureStorageService.GetNetwork();

                _Network = Network.GetNetwork(networkStr);
            }

            if (dryRun) return;

            lock (_Lock)
            {
                // Database cleanup
                // Delete method in FileSystemStorage
                Wallet.Storage.Delete();
            }

            // TODO Make sure that removing all secure storage is the right thing to do
            SecureStorageService.RemoveAll();
        }

        public static bool IsWordInWordlist(string word, string wordList = "english")
        {
            if (string.IsNullOrEmpty(word))
                return false;

            return Hd.IsWordInWordlist(word, wordList);
        }

        public static string[] GenerateGuessWords(string wordToGuess, string language = "english", int amountAround = 9)
        {
            return Hd.GenerateGuessWords(wordToGuess, language, amountAround);
        }

        public static bool IsVerifyChecksum(string mnemonic, string wordList = "english")
        {
            return Hd.IsValidChecksum(mnemonic, wordList);
        }

        public string GetWordListLanguage()
        {
            // TODO This should read from the user's language.
            string language = "english";

            Logger.Information($"Wordlist is on {language}");

            return language;
        }

        public BitcoinAddress GetReceiveAddress()
        {
            return Wallet.CurrentAccount.GetReceiveAddress();
        }

        public long GetCurrentAccountBalanceInSatoshis(bool includeUnconfirmed = false)
        {
            var balance = Wallet.CurrentAccount.GetBalance();

            // TODO Implement includeUnconfirmed condition

            return balance.Satoshi;
        } 

        public decimal GetCurrentAccountBalanceInBTC(bool includeUnconfirmed = false)
        {
            long sats = GetCurrentAccountBalanceInSatoshis(includeUnconfirmed);
            decimal satsPerBtc = 100_000_000m;

            return decimal.Divide(new decimal(sats), satsPerBtc);
        }

        public IEnumerable<Tx> GetCurrentAccountTransactions()
        {
            if (Wallet.CurrentAccount == null) return new List<Tx>();

            return Wallet.CurrentAccount.Txs;
        }

        public (bool Success, Transaction Tx, decimal Fees, string Error) CreateTransaction(decimal amount, string addressTo, long feeSatsPerByte, string password)
        {
            // TODO
            Money btcAmount = new Money(amount, MoneyUnit.BTC);
            Transaction tx = null;
            decimal fees = 0.0m;

            try
            {
                tx = TransactionExtensions.CreateTransaction(
                    password,
                    addressTo,
                    btcAmount,
                    feeSatsPerByte,
                    Wallet,
                    Wallet.CurrentAccount,
                    _Network
                );
                fees = tx.GetVirtualSize() * feeSatsPerByte;

                return (true, tx, fees, null);
            }
            catch (WalletException e)
            {
                Logger.Error(e.Message);

                return (false, tx, fees, e.Message);
            }
        }

        public async Task<(bool Sent, string Error)> SendTransaction(Transaction tx)
        {
            return await Wallet.SendTransaction(tx);
        }

        public Network GetNetwork()
        {
            Guard.NotNull(_Network, nameof(_Network));

            return _Network;
        }

        string GetConfigFile(string fileName)
        {
            string configFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

            Logger?.Information("Getting config file: {configFileName}", configFileName);

            return configFileName;
        }
    }
}
