//
// WalletService.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 HODL Wallet
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
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

using Xamarin.Forms;
using ReactiveUI;
using NBitcoin;

using Liviano;
using Liviano.Bips;
using Liviano.Interfaces;
using Liviano.Utilities;
using Liviano.Extensions;
using Liviano.Models;
using Liviano.Exceptions;

using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Services;

[assembly: Dependency(typeof(WalletService))]
namespace HodlWallet.Core.Services
{
    public sealed class WalletService : IWalletService
    {
        public const int DEFAULT_NODES_TO_CONNECT = 4;

#if DEBUG || TESTNET
        public const string DEFAULT_NETWORK = "testnet";
#else
        public const string DEFAULT_NETWORK = "mainnet";
#endif

        public static string USER_AGENT { get; } = $"{Liviano.Version.UserAgent}/hodlwallet:2.0/";

        public const int PERIODIC_SAVE_TIMEOUT = 30_000;

        const int SYNC_WATCH_DELAY_MS = 420;

        readonly object @lock = new();

        Network network;

        string walletId;
        readonly CancellationTokenSource cts = new();

        public Serilog.ILogger Logger { set; get; }

        public BlockLocator ScanLocation { get; set; }

        public event EventHandler OnConfigured;
        public event EventHandler OnStarted;
        public event EventHandler<DateTimeOffset> OnSyncStarted;
        public event EventHandler<DateTimeOffset> OnSyncFinished;

        public bool IsStarted { get; private set; }
        public bool IsConfigured { get; private set; }
        public bool Syncing { get; private set; }

        public IWallet Wallet { get; private set; }

        IBackgroundService BackgroundService => DependencyService.Get<IBackgroundService>();

        void PeriodicSave()
        {
            Observable
                .Interval(TimeSpan.FromMilliseconds(PERIODIC_SAVE_TIMEOUT), RxApp.TaskpoolScheduler)
                .Subscribe(_ => Save(), cts.Token);
        }

        void Save()
        {
            lock (@lock)
            {
                Wallet.Storage.Save();
            }
        }

        public void InitializeWallet(string accountType = "standard")
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

            string networkStr;
            if (SecureStorageService.HasNetwork())
            {
                networkStr = SecureStorageService.GetNetwork();
            }
            else
            {
                networkStr = DEFAULT_NETWORK;
                SecureStorageService.SetNetwork(networkStr);
            }

            network = Hd.GetNetwork(networkStr ?? DEFAULT_NETWORK);

            walletId = guid ?? Guid.NewGuid().ToString();

            if (!SecureStorageService.HasMnemonic() || walletId == null)
            {
                Logger.Information("Wallet has been configured but not started yet due to the lack of mnemonic in the system");
                return;
            }

            StartWalletWithWalletId(accountType);

            Logger.Information("Since wallet has a mnemonic, then start the wallet.");

            OnConfigured?.Invoke(this, null);
            IsConfigured = true;

            Logger.Information("Configured wallet.");
        }

        public void StartWalletWithWalletId(string newAccountType = "standard")
        {
            Guard.NotNull(walletId, nameof(walletId));

            string mnemonic = SecureStorageService.GetMnemonic();
            string password = ""; // TODO password cannot be null. but it should be
                                  // change liviano load wallet to accept null passwords
                                  // But, since HODLWallet 1 didn't have passwords this is okay

            network = Hd.GetNetwork(DEFAULT_NETWORK);
            var storage = new WalletStorageProvider(walletId, network);

            if (storage.Exists())
            {
                Logger.Information("Loading a wallet because it exists");

                try
                {
                    var err = new WalletException("error");
                    Wallet = storage.Load("", out err);

                    Start();
                    Logger.Information("Wallet started.");

                    return;
                }
                catch (Exception ex)
                {
                    Logger.Information(ex.Message);

                    // What would lead to this?

                    // TODO: Defensive programming is a bad practice, this is a bad practice
                    if (!Hd.IsMnemonicOfWallet(new Mnemonic(mnemonic), Wallet, network))
                    {
                        lock (@lock)
                        {
                            storage.Delete();
                        }
                    }
                }
            }

            Logger.Debug("Creating wallet ({guid}) with password: {password}", walletId, password);

            DateTimeOffset createdAt = new DateTimeOffset(DateTime.UtcNow);

            Wallet = new Wallet { Id = walletId };

            Wallet.Init(mnemonic, password, null, network, createdAt, storage);

            Wallet.AddAccount("bip84");

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

            Wallet.InitElectrumPool();

            // TODO There's an error here, ElectrumPool is null, we need to call the connect code.
            Wallet.ElectrumPool.OnSyncStarted += (obj, _) =>
            {
                start = DateTimeOffset.Now;

                OnSyncStarted?.Invoke(this, start);
                Syncing = true;

                Logger.Debug($"Syncing started at {start.LocalDateTime.ToLongTimeString()}");
            };

            Wallet.ElectrumPool.OnSyncFinished += (obj, _) =>
            {
                end = DateTimeOffset.UtcNow;

                OnSyncFinished?.Invoke(this, end);
                Syncing = false;

                Logger.Debug($"Syncing ended at {end.LocalDateTime.ToLongTimeString()}");
                Logger.Debug($"Syncing time: {(end - start).TotalSeconds}");
            };

            PeriodicSave();

            BackgroundService.Start("PeriodicPingJob", async () =>
            {
                await Wallet.ElectrumPool.PeriodicPing(
                    successCallback: (dt) =>
                    {
                        Debug.WriteLine($"[WalletService][Start] Ping successful at: {dt}");
                    },
                    failedCallback: (dt) =>
                    {
                        Debug.WriteLine($"[WalletService][Start] Ping failed at: {dt}");
                    },
                    null
                );
            });

            BackgroundService.Start("SyncWatchJob", async () =>
            {
                await Wallet.Sync();
                await Task.Delay(SYNC_WATCH_DELAY_MS);
                await Wallet.Watch();
            });

            IsStarted = true;
            OnStarted?.Invoke(this, null);
        }

        public (bool Success, string Error) AddAccount(string type = "bip84", string name = null, string color = null)
        {
            bool addedAccount = false;
            string messageError = null;
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    messageError = "Invalid account name: It cannot be empty!";
                    Logger.Error(messageError);
                }
                else
                {
                    Wallet.AddAccount(type, name);

                    // To ensure the account is correctly associated to the current Wallet configured.
                    Save();

                    string accountIdSaved =
                            (from account in Wallet.Accounts
                             where account.Name == name
                             select account.Id).LastOrDefault();

                    if (string.IsNullOrWhiteSpace(accountIdSaved))
                    {
                        messageError = $"Unable to add a new account to the current Wallet | Name => {name} - Type => {type}.";
                        Logger.Error(messageError);
                    }
                    else
                    {
                        addedAccount = true;

                        // Backup the color associated with this account
                        SetAccountColor(accountIdSaved, color);
                    }
                }
                return (addedAccount, messageError);
            }
            catch (WalletException e)
            {
                Logger.Error(e.Message);

                return (addedAccount, e.Message);
            }
        }

        void SetAccountColor(string accountId, string color)
        {
            Guard.NotEmpty(color, nameof(color));
            SecureStorageService.SetAccountColor(walletId, accountId, color);
        }

        public string GetColorByAccount(string accountId)
        {
            Guard.NotEmpty(accountId, nameof(accountId));
            string color = SecureStorageService.GetAccountColor(walletId, accountId);
            return color;
        }

        public static string GetNewMnemonic(string wordList = "english", int wordCount = 12)
        {
            return Hd.NewMnemonic(wordList, wordCount).ToString();
        }

        public bool IsAddressOwn(string address)
        {
            var parsedAddress = BitcoinAddress.Create(address, network);

            bool inInternal = Wallet.CurrentAccount.UsedInternalAddresses
                              .Contains(parsedAddress) ||
                              parsedAddress == Wallet.CurrentAccount.GetReceiveAddress();

            bool inExternal = Wallet.CurrentAccount.UsedExternalAddresses
                              .Contains(parsedAddress);

            return inInternal || inExternal;
        }

        /// <summary>
        /// Destroy wallet, deletes wallet files
        /// </summary>
        /// <param name="dryRun">Do not delete anything just try</param>
        public void DestroyWallet(bool dryRun = false)
        {
            var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}/wallets";

            if (dryRun)
            {
                Debug.WriteLine($"[DestroyWallet] Would delete `{path}`... but dry");

                return;
            }

            if (Directory.Exists(path)) Directory.Delete(path, true);

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

            Logger.Information($"[GetWordListLanguage] Wordlist is on {language}");

            return language;
        }

        public BitcoinAddress GetReceiveAddress()
        {
            return Wallet.CurrentAccount.GetReceiveAddress();
        }

        public long GetCurrentAccountBalanceInSatoshis(bool includeUnconfirmed = false)
        {
            return GetCurrentAccountBalanceInBTC(includeUnconfirmed).Satoshi;
        }

        public Money GetCurrentAccountBalanceInBTC(bool includeUnconfirmed = false)
        {
            return Wallet.CurrentAccount.GetBalance();
        }

        public IEnumerable<Tx> GetCurrentAccountTransactions()
        {
            if (Wallet.CurrentAccount == null) return new List<Tx>();

            return Wallet.CurrentAccount.Txs;
        }

        public (bool Success, Transaction Tx, decimal Fees, string Error) CreateTransaction(decimal amount, string addressTo, decimal feeSatsPerByte, string password)
        {
            // TODO
            Money btcAmount = new(amount, MoneyUnit.BTC);
            Transaction tx = null;
            decimal fees = 0.0m;

            try
            {
                string error;
                (tx, error) = Wallet.CurrentAccount.CreateTransaction(
                    addressTo,
                    btcAmount,
                    feeSatsPerByte,
                    true
                );
                fees = tx.GetVirtualSize() * feeSatsPerByte;
                bool verified = string.IsNullOrEmpty(error);

                return (verified, tx, fees, error);
            }
            catch (WalletException e)
            {
                Logger.Error(e.Message);

                return (false, tx, fees, e.Message);
            }
        }

        public async Task<(bool Sent, string Error)> SendTransaction(Transaction tx)
        {
            return await Wallet.Broadcast(tx);
        }

        public Network GetNetwork()
        {
            Guard.NotNull(network, nameof(network));

            return network;
        }

        //string GetConfigFile(string fileName)
        //{
        //    string configFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

        //    Logger?.Information("Getting config file: {configFileName}", configFileName);

        //    return configFileName;
        //}
    }
}
