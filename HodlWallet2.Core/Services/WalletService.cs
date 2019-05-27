using System;
using Liviano.Managers;
using Serilog;

using System.Linq;

using NBitcoin;
using NBitcoin.Protocol;

using Liviano;
using Liviano.Interfaces;
using Liviano.Utilities;

using NBitcoin.Protocol.Behaviors;

using System.IO;
using System.Threading.Tasks;

using Liviano.Behaviors;
using Liviano.Enums;
using Liviano.Models;
using System.Collections.Generic;
using HodlWallet2.Core.Interfaces;
using Liviano.Exceptions;
using MvvmCross.Logging;
using MvvmCross;

using HodlWallet2.Core.Utils;

namespace HodlWallet2.Core.Services
{
    public sealed class WalletService : IWalletService
    {
        public const int DEFAULT_NODES_TO_CONNECT = 4;

        public const string DEFAULT_NETWORK = "main";

        public static readonly string USER_AGENT = $"{Liviano.Version.UserAgent}/hodlwallet:2.0/";

        private static readonly object _SingletonLock = new object();

        private object _Lock = new object();

        private int _NodesToConnect;

        private NodeConnectionParameters _conParams;

        private Network _Network;

        private AddressManager _AddressManager;

        private PartialConcurrentChain _Chain;

        private DefaultCoinSelector _DefaultCoinSelector;

        private NodesGroup _NodesGroup;

        private WalletSyncManagerBehavior _WalletSyncManagerBehavior;

        private string _WalletId;

        public Serilog.ILogger Logger { set; get; }

        public WalletManager WalletManager { get; set; }

        public IBroadcastManager BroadcastManager { get; set; }

        public ITransactionManager TransactionManager { get; set; }

        public IAsyncLoopFactory AsyncLoopFactory { get; set; }

        public IDateTimeProvider DateTimeProvider { get; set; }

        public IScriptAddressReader ScriptAddressReader { get; set; }

        public IStorageProvider StorageProvider { get; set; }

        public IWalletSyncManager WalletSyncManager { get; set; }

        public NodesGroup NodesGroup { get; set; }

        public BlockLocator ScanLocation { get; set; }

        public event EventHandler OnConfigured;
        public event EventHandler OnStarted;
        public event EventHandler OnScanning;

        public bool IsStarted { get; set; }
        public bool IsConfigured { get; set; }

        public static WalletService Instance
        {
            get
            {
                return (WalletService) Mvx.IoCProvider.Resolve<IWalletService>();
            }
        }

        public HdAccount CurrentAccount
        {
            get
            {
                // FIXME Please change this method once accounts are implemented.
                //       That means people will change this manually by clicking on a
                //       different account.
                return WalletManager.GetWallet().GetAccountsByCoinType(CoinType.Bitcoin).FirstOrDefault();
            }

            set
            {
                throw new NotImplementedException("Please code this.");
            }
        }

        private PartialConcurrentChain GetChain()
        {
            lock (_Lock)
            {
                if (_conParams != null)
                {
                    return _conParams.TemplateBehaviors.Find<PartialChainBehavior>().Chain as PartialConcurrentChain;
                }

                var chain = new PartialConcurrentChain(_Network);

                using (var fs = File.Open(ChainFile(), FileMode.OpenOrCreate))
                {
                    chain.Load(new BitcoinStream(fs, false));
                }

                if (chain.Tip.Height < _Network.GetBIP39ActivationChainedBlock().Height)
                    chain.SetCustomTip(_Network.GetBIP39ActivationChainedBlock());

                return chain;
            }
        }


        private string GetConfigFile(string fileName)
        {
            string configFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

            Logger?.Information("Getting config file: {configFileName}", configFileName);

            return configFileName;
        }

        private string AddrmanFile()
        {
            Guard.NotNull(_Network, nameof(_Network));

            return GetConfigFile($"addrman-{_Network.Name.ToLower()}.dat");
        }

        private string ChainFile()
        {
            Guard.NotNull(_Network, nameof(_Network));

            return GetConfigFile($"chain-{_Network.Name.ToLower()}.dat");
        }

        private async Task PeriodicSave()
        {
            while (true)
            {
                await Save();

                Logger.Information("Saved chain file to filepath: {filepath} on {now}", ChainFile(), DateTime.Now);

                int delay = 50_000;

                await Task.Delay(delay);
            }
        }

        private async Task Save()
        {
            await Task.Factory.StartNew(() =>
            {
                lock (_Lock)
                {
                    _AddressManager.SavePeerFile(AddrmanFile(), _Network);

                    using (var fs = File.Open(ChainFile(), FileMode.OpenOrCreate))
                    {
                        PartialConcurrentChain chain = GetChain();
                        chain.WriteTo(new BitcoinStream(fs, true));
                    }
                }
            });
        }

        private AddressManager GetAddressManager()
        {
            if (_conParams != null)
            {
                return _conParams.TemplateBehaviors.Find<AddressManagerBehavior>().AddressManager;

            }

            if (File.Exists(AddrmanFile()))
            {
                return AddressManager.LoadPeerFile(AddrmanFile(), _Network);
            }
            else
            {
                return new AddressManager();
            }
        }

        private ChainedBlock GetClosestChainedBlockToDateTimeOffset(DateTimeOffset? creationDate)
        {
            Guard.NotNull(_Network, nameof(_Network));
            long ticks = 0;

            if (creationDate.HasValue)
                ticks = creationDate.Value.Ticks;

            return _Network.GetCheckpoints().OrderBy(chainedBlock => Math.Abs(chainedBlock.Header.BlockTime.Ticks - ticks)).FirstOrDefault();
        }
        
        public WalletService()
        {
            
        }

        public void InitializeWallet()
        {
            string guid;
            if (SecureStorageProvider.HasWalletId())
            {
                guid = SecureStorageProvider.GetWalletId();
            }
            else
            {
                guid = Guid.NewGuid().ToString();

                SecureStorageProvider.SetWalletId(guid);
            }
            
            // TODO Please store and run the network the user is using.
            //Wallet.Configure(walletId: "wallet_guid", network: "testnet", nodesToConnect: 4);
            Configure(walletId: guid, network: "testnet");

            // FIXME Remove this code later when we have a way to create a wallet,
            // for now, the wallet is created and hardcoded
            string mnemonic = "erase fog enforce rice coil start few hold grocery lock youth service among menu life salmon fiction diamond lyrics love key stairs toe transfer";
            string password = "123456";

            if (!WalletExists())
            {
                Logger.Information("Creating wallet ({guid}) with password: {password}", guid, password);

                WalletManager.CreateWallet(guid, password, WalletManager.MnemonicFromString(mnemonic));

                Logger.Information("Wallet created.");
            }

            // NOTE Do not delete this, this is correct, the wallet should start after it being configured.
            //      Also change the date, the argument should be avoided.
            Start(password, new DateTimeOffset(new DateTime(2014, 12, 1)));

            Logger.Information("Wallet started.");
        }

        public static string GetNewMnemonic(string wordlist = "english", int wordcount = 12)
        {
            return new Mnemonic(HdOperations.WordlistFromString(wordlist), HdOperations.WordCountFromInt(wordcount)).ToString();
        }

        public static IEnumerable<HdAddress> GetAddressesFromTransaction(TransactionData txData)
        {
            return WalletService.Instance.CurrentAccount.FindAddressesForTransaction(tx => tx.Id == txData.Id);
        }

        public static string GetAddressFromTranscation(TransactionData txData)
        {
            var addrsFromTx = GetAddressesFromTransaction(txData).Select(hdAddress => hdAddress.Address);

            if (txData.IsReceive == true)
            {
                return WalletService.Instance.CurrentAccount.ExternalAddresses.First(
                    externalAddress => addrsFromTx.Contains(externalAddress.Address)
                ).Address;
            }
            else if (txData.IsSend == true) // For verbosity and error catching...
            {
                return WalletService.Instance.CurrentAccount.InternalAddresses.First(
                    internalAddress => addrsFromTx.Contains(internalAddress.Address)
                ).Address;
            }
            else
            {
                throw new WalletException("Tx data isn't send or receive, something is wrong...");
            }
        }

        public void Configure(string walletId = null, string network = null, int? nodesToConnect = null)
        {
            _Network = HdOperations.GetNetwork(network ?? DEFAULT_NETWORK);
            _Chain = GetChain();
            _AddressManager = GetAddressManager();
            _NodesToConnect = nodesToConnect ?? DEFAULT_NODES_TO_CONNECT;
            _WalletId = walletId ?? Guid.NewGuid().ToString();
            _conParams = new NodeConnectionParameters();

            Logger.Information("Running on {network}", _Network.Name);
            Logger.Information("With wallet id: {walletId}", _WalletId);
            Logger.Information("Will try to connect to {nodesToConnect}", _NodesToConnect);

            DateTimeProvider = new DateTimeProvider();
            AsyncLoopFactory = new AsyncLoopFactory();
            ScriptAddressReader = new ScriptAddressReader();
            StorageProvider = new WalletStorageProvider(_WalletId);

            if (!StorageProvider.WalletExists())
            {
                Logger.Information("Will create a new wallet {walletId} since it doesn't exists", _WalletId);
            }

            WalletManager = new WalletManager(Logger, _Network, _Chain, AsyncLoopFactory, DateTimeProvider, ScriptAddressReader, StorageProvider);
            WalletSyncManager = new WalletSyncManager(Logger, WalletManager, _Chain);

            _WalletSyncManagerBehavior = new WalletSyncManagerBehavior(Logger, WalletSyncManager, ScriptTypes.P2WPKH);

            _conParams.TemplateBehaviors.Add(new AddressManagerBehavior(_AddressManager));
            _conParams.TemplateBehaviors.Add(new PartialChainBehavior(_Chain, _Network) { CanRespondToGetHeaders = false, SkipPoWCheck = true });
            _conParams.TemplateBehaviors.Add(_WalletSyncManagerBehavior);

            _conParams.UserAgent = USER_AGENT;

            _NodesGroup = new NodesGroup(_Network, _conParams, new NodeRequirement()
            {
                RequiredServices = NodeServices.Network
            });

            Logger.Information("Requiring service 'NETWORK' for SPV");

            BroadcastManager broadcastManager = new BroadcastManager(_NodesGroup);

            _conParams.TemplateBehaviors.Add(new TransactionBroadcastBehavior(broadcastManager));

            _NodesGroup.NodeConnectionParameters = _conParams;
            _NodesGroup.MaximumNodeConnection = _NodesToConnect;

            _DefaultCoinSelector = new DefaultCoinSelector();

            Logger.Information("Coin selector: {coinSelector}", _DefaultCoinSelector.GetType().ToString());

            TransactionManager = new TransactionManager(broadcastManager, WalletManager, _DefaultCoinSelector, _Chain);

            Logger.Information("Add transaction manager.");

            Logger.Information("Configured wallet.");

            OnConfigured?.Invoke(this, null);
            IsConfigured = true;
        }

        public void Start(string password, DateTimeOffset? timeToStartOn = null)
        {
            WalletManager.LoadWallet(password);

            _NodesGroup.Connect();

            WalletManager.Start();

            Scan(timeToStartOn);

            _ = PeriodicSave();

            OnStarted?.Invoke(this, null);
            IsStarted = true;
        }

        public void Scan(DateTimeOffset? timeToStartOn)
        {
            ScanLocation = new BlockLocator();
            ICollection<uint256> walletBlockLocator = WalletManager.GetWalletBlockLocator();
            ChainedBlock closestChainedBlock = GetClosestChainedBlockToDateTimeOffset(timeToStartOn);

            // If there are block locations in the wallet
            if (walletBlockLocator != null && walletBlockLocator.Count > 0)
            {
                ScanLocation.Blocks.AddRange(walletBlockLocator);
            }
            else
            {
                // Add genesis
                ScanLocation = _Network.GetDefaultBlockLocator();
            }

            if (_Chain.Tip.Height < closestChainedBlock.Height)
                _Chain.SetCustomTip(closestChainedBlock);

            WalletSyncManager.Scan(ScanLocation, closestChainedBlock.Header.BlockTime);

            OnScanning?.Invoke(this, null);
        }

        public void ReScan(DateTimeOffset? timeToStartOn)
        {
            // FIXME this method dosn't work crashes on Scan.
            throw new NotImplementedException("Please finish work here");

            string chainFile = ChainFile();
            string addrmanFile = AddrmanFile();
            string walletFile = ((WalletStorageProvider)StorageProvider).FilePath;
            DateTimeOffset currentCreationTime = WalletManager.GetWallet().CreationTime;

            // Database cleanup
            Logger.Information("Deleting chain file: {chainFile}", chainFile);
            File.Delete(chainFile);

            Logger.Information("Deleting address manager file: {addrmanFile}", addrmanFile);
            File.Delete(addrmanFile);

            Logger.Information("Deleting wallet file: {walletFile}", walletFile);
            File.Delete(walletFile);

            // Create wallet
            // FIXME: This should not be done like this.
            //        Wallet should be created but with data we already have on SecureStorageProvider for mnemonic and password.
            string guid = "736083c0-7f11-46c2-b3d7-e4e88dc38889";
            string mnemonic = "erase fog enforce rice coil start few hold grocery lock youth service among menu life salmon fiction diamond lyrics love key stairs toe transfer";
            string password = "123456";

            Logger.Information("Unloaded wallet");
            WalletManager.UnloadWallet();

            Logger.Information("Creating wallet ({guid}) with password: {password}", guid, password);
            WalletManager.CreateWallet(guid, password, WalletManager.MnemonicFromString(mnemonic));

            Logger.Information("Wallet created.");

            Logger.Information("Wallet re-loaded");
            WalletManager.LoadWallet(password);

            WalletManager.GetWallet().CreationTime = currentCreationTime;

            WalletManager.SaveWallet(WalletManager.GetWallet());

            // Nodes reconnect if it's not null
            NodesGroup?.Disconnect();
            NodesGroup?.Connect();

            // Start scanning again
            Scan(timeToStartOn);
        }

        public bool WalletExists()
        {
            if (_WalletId == null)
                return false;

            return StorageProvider.WalletExists();
        }

        public string NewMnemonic(string wordList = "english", int wordCount = 12)
        {
            return WalletManager.NewMnemonic(wordList, wordCount).ToString();
        }

        public bool IsWordInWordlist(string word, string wordList = "english")
        {
            if (string.IsNullOrEmpty(word))
                return false;

            return HdOperations.IsWordInWordlist(word, wordList);
        }

        public string[] GenerateGuessWords(string wordToGuess, string language = "english", int amountAround = 9)
        {
            return HdOperations.GenerateGuessWords(wordToGuess, language, amountAround);
        }

        public bool IsVerifyChecksum(string mnemonic, string wordList = "english")
        {
            return HdOperations.IsValidChecksum(mnemonic, wordList);
        }

        public HdAddress GetReceiveAddress()
        {
            return CurrentAccount.GetFirstUnusedReceivingAddress();
        }

        public IEnumerable<TransactionData> GetCurrentAccountTransactions()
        {
            IEnumerable<TransactionData> result = new List<TransactionData>();

            if (WalletManager != null)
            {
                result = WalletManager.GetAllAccountsByCoinType(CoinType.Bitcoin)
                        .SelectMany((HdAccount account) => account.GetCombinedAddresses())
                        .SelectMany((HdAddress address) => address.Transactions);
            }

            return result;
        }

        public (bool Success, Transaction Tx, decimal Fees, string Error) CreateTransaction(decimal amount, string addressTo, int feeSatsPerByte, string password)
        {
            Money btcAmount = new Money(amount, MoneyUnit.BTC);
            Transaction tx = null;
            decimal fees = 0.0m;

            try
            {
                tx = TransactionManager.CreateTransaction(
                    addressTo,
                    btcAmount,
                    feeSatsPerByte,
                    CurrentAccount,
                    password,
                    signTransaction: true
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
            try
            {
                await TransactionManager.BroadcastTransaction(tx);
                return (true, null);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);

                return (false, e.Message);
            }
        }
    }
}
