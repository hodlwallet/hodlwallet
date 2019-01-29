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
using Liviano.Exceptions;

using HodlWallet2.Utils;

namespace HodlWallet2
{
    public sealed class Wallet
    {
        public const int DEFAULT_NODES_TO_CONNECT = 4;

        public const string DEFAULT_NETWORK = "main";

        private static Wallet instance = null;

        private static readonly object _SingletonLock = new object();

        private object _Lock = new object();

        private int _NodesToConnect;

        private NodeConnectionParameters _ConParams;

        private Network _Network;

        private AddressManager _AddressManager;

        private ConcurrentChain _Chain;

        private DefaultCoinSelector _DefaultCoinSelector;

        private NodesGroup _NodesGroup;

        private WalletSyncManagerBehavior _WalletSyncManagerBehavior;

        private string _WalletId;

        public ILogger Logger { set; get; }

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

        public EventHandler OnConfigured;
        public EventHandler OnStarted;
        public EventHandler OnScanning;

        public bool Started = false;
        public bool Configured = false;

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

        private ConcurrentChain GetChain()
        {
            lock (_Lock)
            {
                if (_ConParams != null)
                {
                    return _ConParams.TemplateBehaviors.Find<PartialChainBehavior>().Chain as PartialConcurrentChain;
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

        private static string GetConfigFile(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
        }

        private static string AddrmanFile()
        {
            return GetConfigFile("addrman.dat");
        }

        private static string ChainFile()
        {
            return GetConfigFile("chain.dat");
        }

        private static async Task PeriodicSave()
        {
            while (true)
            {
                await Instance.Save();

                Instance.Logger.Information("Saved chain file to filepath: {filepath} on {now}", ChainFile(), DateTime.Now);

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
                        _Chain.WriteTo(fs);
                    }
                }
            });
        }

        private AddressManager GetAddressManager()
        {
            if (_ConParams != null)
            {
                return _ConParams.TemplateBehaviors.Find<AddressManagerBehavior>().AddressManager;

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

        Wallet()
        {
        }

        public static Wallet Instance
        {
            get
            {
                lock (_SingletonLock)
                {
                    if (instance == null)
                    {
                        instance = new Wallet();
                    }

                    return instance;
                }
            }
        }

        public static void InitializeWallet()
        {
            // FIXME Remove this with the removable code bellow.
            string guid = "736083c0-7f11-46c2-b3d7-e4e88dc38889";

            // TODO Please store and run the network the user is using.
            //Wallet.Configure(walletId: "wallet_guid", network: "testnet", nodesToConnect: 4);
            Instance.Configure(walletId: guid, network: "testnet");

            // FIXME Remove this code later when we have a way to create a wallet,
            // for now, the wallet is created and hardcoded
            string mnemonic = "erase fog enforce rice coil start few hold grocery lock youth service among menu life salmon fiction diamond lyrics love key stairs toe transfer";
            string password = "123456";

            if (!Instance.WalletExists())
            {
                Instance.Logger.Information("Creating wallet ({guid}) with password: {password}", guid, password);

                Instance.WalletManager.CreateWallet(guid, password, WalletManager.MnemonicFromString(mnemonic));

                Instance.Logger.Information("Wallet created.");
            }

            // NOTE Do not delete this, this is correct, the wallet should start after it being configured.
            //      Also change the date, the argument should be avoided.
            Instance.Start(password, new DateTimeOffset(new DateTime(2018, 12, 1)));

            Instance.Logger.Information("Wallet started.");
        }

        public void Configure(string walletId = null, string network = null, int? nodesToConnect = null)
        {
            _Network = HdOperations.GetNetwork(network ?? DEFAULT_NETWORK);
            _Chain = GetChain();
            _AddressManager = GetAddressManager();
            _NodesToConnect = nodesToConnect ?? DEFAULT_NODES_TO_CONNECT;
            _WalletId = walletId ?? Guid.NewGuid().ToString();
            _ConParams = new NodeConnectionParameters();

            Logger.Information("Running on {network}", _Network.Name);
            Logger.Information("With wallet id: {walletId}", _WalletId);
            Logger.Information("Will try to connect to {nodesToConnect}", _NodesToConnect);

            DateTimeProvider = new DateTimeProvider();
            AsyncLoopFactory = new AsyncLoopFactory();
            ScriptAddressReader = new ScriptAddressReader();
            StorageProvider = new StorageProvider(_WalletId);

            if (!StorageProvider.WalletExists())
            {
                Logger.Information("Creating a new wallet {walletId}", _WalletId);
            }

            WalletManager = new WalletManager(Logger, _Network, _Chain, AsyncLoopFactory, DateTimeProvider, ScriptAddressReader, StorageProvider);
            WalletSyncManager = new WalletSyncManager(Logger, WalletManager, _Chain);

            _WalletSyncManagerBehavior = new WalletSyncManagerBehavior(Logger, WalletSyncManager, ScriptTypes.SegwitAndLegacy);

            _ConParams.TemplateBehaviors.Add(new AddressManagerBehavior(_AddressManager));
            _ConParams.TemplateBehaviors.Add(new PartialChainBehavior(_Chain, _Network));
            _ConParams.TemplateBehaviors.Add(_WalletSyncManagerBehavior);

            _ConParams.UserAgent = "/hodlwallet:2.0/";

            _NodesGroup = new NodesGroup(_Network, _ConParams, new NodeRequirement()
            {
                RequiredServices = NodeServices.Network
            });

            Logger.Information("Requiring service 'NETWORK' for SPV");

            BroadcastManager broadcastManager = new BroadcastManager(_NodesGroup);

            _ConParams.TemplateBehaviors.Add(new TransactionBroadcastBehavior(broadcastManager));

            _NodesGroup.NodeConnectionParameters = _ConParams;
            _NodesGroup.MaximumNodeConnection = _NodesToConnect;

            _DefaultCoinSelector = new DefaultCoinSelector();

            Logger.Information("Coin selector: {coinSelector}", _DefaultCoinSelector.GetType().ToString());

            TransactionManager = new TransactionManager(broadcastManager, WalletManager, _DefaultCoinSelector, _Chain);

            Logger.Information("Add transaction manager.");

            Logger.Information("Configured wallet.");

            OnConfigured?.Invoke(this, null);
            Configured = true;
        }

        public void Start(string password, DateTimeOffset? timeToStartOn = null)
        {
            WalletManager.LoadWallet(password);

            _NodesGroup.Connect();

            WalletManager.Start();

            Scan(timeToStartOn);

            PeriodicSave();

            OnStarted?.Invoke(this, null);
            Started = true;
        }

        public void Scan(DateTimeOffset? timeToStartOn)
        {
            ScanLocation = new BlockLocator();
            ICollection<uint256> walletBlockLocator = WalletManager.GetWalletBlockLocator();

            if (walletBlockLocator != null)
            {
                ScanLocation.Blocks.AddRange(walletBlockLocator);
            }

            // Regardless we always add the checkpoints.
            ScanLocation.Blocks.Add(_Network.GetBIP39ActivationChainedBlock().HashBlock);
            // Then the checkpoints
            ScanLocation.Blocks.AddRange(
                _Network.GetCheckpoints().Select(checkpoint => checkpoint.HashBlock)
            );

            if (timeToStartOn == null)
            {
                ChainedBlock lastReceivedBlock = _Chain.GetBlock(WalletManager.LastReceivedBlockHash() ?? (uint)_Chain.Tip.Height);

                if (lastReceivedBlock != null)
                {
                    timeToStartOn = lastReceivedBlock.Header.BlockTime;
                }
                else
                {
                    if (WalletManager.GetWalletCreationTime() < _Chain.Tip.Header.BlockTime)
                    {
                        timeToStartOn = _Chain.Tip.Header.BlockTime;
                    }
                    else
                    {
                        timeToStartOn = WalletManager.GetWalletCreationTime();
                    }
                }
            }

            WalletSyncManager.Scan(ScanLocation, timeToStartOn.Value);

            OnScanning?.Invoke(this, null);
        }

        public void ReScan(DateTimeOffset? timeToStartOn)
        {
            string chainFile = ChainFile();
            string addrmanFile = AddrmanFile();
            string walletFile = ((StorageProvider)StorageProvider).FilePath;

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
            WalletManager.LoadWallet();
            
            // Nodes reconnect
            NodesGroup.Disconnect();
            NodesGroup.Connect();

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
