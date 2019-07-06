using System;
using System.Collections.Generic;
using System.Security;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using MvvmCross;

using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.Protocol.Behaviors;

using Liviano;
using Liviano.Interfaces;
using Liviano.Utilities;
using Liviano.Behaviors;
using Liviano.Enums;
using Liviano.Models;
using Liviano.Managers;
using Liviano.Exceptions;

using HodlWallet2.Core.Interfaces;

namespace HodlWallet2.Core.Services
{
    public sealed class WalletService : IWalletService
    {
        public const int DEFAULT_NODES_TO_CONNECT = 4;

        public const string DEFAULT_NETWORK = "testnet";

        public static readonly string UserAgent = $"{Liviano.Version.UserAgent}/hodlwallet:2.0/";

        object _Lock = new object();

        int _NodesToConnect;

        NodeConnectionParameters _ConParams;

        Network _Network;

        AddressManager _AddressManager;

        PartialConcurrentChain _Chain;

        DefaultCoinSelector _DefaultCoinSelector;

        NodesGroup _NodesGroup;

        WalletSyncManagerBehavior _WalletSyncManagerBehavior;

        string _WalletId;

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

        int _ConnectedNodes;
        public int ConnectedNodes
        {
            get
            {
                return _ConnectedNodes;
            }

            set
            {
                _ConnectedNodes = value;
                OnConnectedNode.Invoke(this, _ConnectedNodes);
            }
        }

        public BlockLocator ScanLocation { get; set; }

        public event EventHandler OnConfigured;
        public event EventHandler OnStarted;
        public event EventHandler OnScanning;
        public event EventHandler<int> OnConnectedNode;

        public bool IsStarted { get; private set; }
        public bool IsConfigured { get; private set; }

        public static WalletService Instance => (WalletService)Mvx.IoCProvider.Resolve<IWalletService>();

        /// <summary>
        /// Empty constructor that MvvmCross needs to start as a service
        /// </summary>
        public WalletService() { }

        public HdAccount CurrentAccount
        {
            get
            {
                // FIXME Please change this method once accounts are implemented.
                //       That means people will change this manually by clicking on a
                //       different account.
                return WalletManager.GetWallet().GetAccountsByCoinType(CoinType.Bitcoin).FirstOrDefault();
            }

            set => throw new NotImplementedException("This should switch the current account.");
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
                // Default uses the Guid class from System in .NET
                guid = Guid.NewGuid().ToString();

                SecureStorageProvider.SetWalletId(guid);
            }

            string network;
            if (SecureStorageProvider.HasNetwork())
            {
                network = SecureStorageProvider.GetNetwork();
            }
            else
            {
                network = DEFAULT_NETWORK;
                SecureStorageProvider.SetNetwork(network);
            }

            Configure(walletId: guid, network: network);

            if (SecureStorageProvider.HasMnemonic() && _WalletId != null)
            {
                StartWalletWithWalletId();

                Logger.Information("Since wallet has a mnemonic, then start the wallet.");

                return;
            }

            Logger.Information("Wallet has been configured but not started yet due to the lack of mnemonic in the system");
        }

        public void StartWalletWithWalletId()
        {
            Guard.NotNull(_WalletId, nameof(_WalletId));

            string mnemonic = SecureStorageProvider.GetMnemonic();
            string password = ""; // TODO password cannot be null. but it should be
                                  // change liviano load wallet to accept null passwords
                                  // But, since HODLWallet 1 didn't have passwords this is okay

            if (WalletExists())
            {
                Logger.Information("Loading a wallet because it exists");

                try
                {
                    WalletManager.LoadWallet(password);
                }
                catch (SecurityException ex)
                {
                    Logger.Information(ex.Message);

                    // TODO: Defensive programming is a bad practice, this is a bad practice
                    if (!HdOperations.IsMnemonicOfWallet(mnemonic, WalletManager.GetWallet()))
                    {
                        WalletManager.GetStorageProvider().DeleteWallet();

                        string language = "english";
                        int wordCount = 12;
                        DateTimeOffset createdAt = SecureStorageProvider.HasSeedBirthday()
                            ? DateTimeOffset.FromUnixTimeSeconds(SecureStorageProvider.GetSeedBirthday())
                            : new DateTimeOffset(DateTime.UtcNow);

                        WalletManager.CreateWallet(
                            _WalletId,
                            password,
                            WalletManager.MnemonicFromString(mnemonic),
                            language,
                            wordCount,
                            createdAt
                        );

                        Logger.Information("Wallet created.");
                    }
                }
            }
            else
            {
                Logger.Debug("Creating wallet ({guid}) with password: {password}", _WalletId, password);

                string language = "english";
                int wordCount = 12;
                DateTimeOffset createdAt = SecureStorageProvider.HasSeedBirthday()
                    ? DateTimeOffset.FromUnixTimeSeconds(SecureStorageProvider.GetSeedBirthday())
                    : new DateTimeOffset(DateTime.UtcNow);

                WalletManager.CreateWallet(
                    _WalletId,
                    password,
                    WalletManager.MnemonicFromString(mnemonic),
                    language,
                    wordCount,
                    createdAt
                );

                Logger.Information("Wallet created.");
            }

            // NOTE Do not delete this, this is correct, the wallet should start after it being configured.
            Start(password, WalletManager.GetWallet().CreationTime);

            Logger.Information("Wallet started.");
        }

        public static string GetNewMnemonic(string wordlist = "english", int wordcount = 12)
        {
            return new Mnemonic(HdOperations.WordlistFromString(wordlist), HdOperations.WordCountFromInt(wordcount)).ToString();
        }

        public static IEnumerable<HdAddress> GetAddressesFromTransaction(TransactionData txData)
        {
            return Instance.CurrentAccount.FindAddressesForTransaction(tx => tx.Id == txData.Id);
        }

        public string GetAddressFromTransaction(TransactionData txData)
        {
            var addrsFromTx = GetAddressesFromTransaction(txData).Select(hdAddress => hdAddress.Address);

            if (txData.IsReceive == true)
            {
                return CurrentAccount.ExternalAddresses.First(
                    externalAddress => addrsFromTx.Contains(externalAddress.Address)
                ).Address;
            }

            if (txData.IsSend == true) // For verbosity and error catching...
            {
                return CurrentAccount.InternalAddresses.First(
                    internalAddress => addrsFromTx.Contains(internalAddress.Address)
                ).Address;
            }

            throw new WalletException("Tx data isn't send or receive, something is wrong...");
        }

        public bool IsAddressOwn(string address)
        {
            bool inInternal = CurrentAccount.InternalAddresses.Select(
                    (HdAddress hdAddress) => hdAddress.Address)
                .Contains(address);
            bool inExternal = CurrentAccount.ExternalAddresses.Select(
                    (HdAddress hdAddress) => hdAddress.Address)
                .Contains(address);

            return inInternal || inExternal;
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
            StorageProvider = new WalletStorageProvider(_WalletId);

            if (!StorageProvider.WalletExists())
            {
                Logger.Information("Will create a new wallet {walletId} since it doesn't exists", _WalletId);
            }

            WalletManager = new WalletManager(Logger, _Network, _Chain, AsyncLoopFactory, DateTimeProvider, ScriptAddressReader, StorageProvider);
            WalletSyncManager = new WalletSyncManager(Logger, WalletManager, _Chain);

            _WalletSyncManagerBehavior = new WalletSyncManagerBehavior(Logger, WalletSyncManager, ScriptTypes.P2WPKH);

            _ConParams.TemplateBehaviors.Add(new AddressManagerBehavior(_AddressManager));
            _ConParams.TemplateBehaviors.Add(new PartialChainBehavior(_Chain, _Network) { CanRespondToGetHeaders = false, SkipPoWCheck = true });
            _ConParams.TemplateBehaviors.Add(_WalletSyncManagerBehavior);

            _ConParams.UserAgent = UserAgent;

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
            IsConfigured = true;
        }

        public void Start(string password, DateTimeOffset? timeToStartOn = null)
        {
            if (WalletManager.GetWallet() == null)
            {
                WalletManager.LoadWallet(password);

                timeToStartOn = WalletManager.GetWallet().CreationTime;
            }

            _NodesGroup.Connect();
            AddNodesGroupEvents();

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

        /// <summary>
        /// Destroy wallet, deletes wallets file and disconnects from nodes
        /// </summary>
        /// <param name="dryRun">Do not delete anything just try</param>
        public void DestroyWallet(bool dryRun = false)
        {
            if (_Network == null)
            {
                string networkStr = SecureStorageProvider.GetNetwork();

                _Network = Network.GetNetwork(networkStr);
            }

            if (StorageProvider == null)
            {
                string walletId = SecureStorageProvider.GetWalletId();

                StorageProvider = new WalletStorageProvider(id: walletId);
            }

            string chainFile = ChainFile();
            string addrmanFile = AddrmanFile();
            string walletFile = ((WalletStorageProvider)StorageProvider).FilePath;

            Logger.Information("Deleting chain file: {chainFile}", chainFile);
            Logger.Information("Deleting address manager file: {addrmanFile}", addrmanFile);
            Logger.Information("Deleting wallet file: {walletFile}", walletFile);

            if (dryRun) return;

            // Disconnect
            _NodesGroup.Disconnect();

            lock (_Lock)
            {
                // Database cleanup
                File.Delete(chainFile);
                File.Delete(addrmanFile);
                File.Delete(walletFile);
            }

            // TODO Make sure that removing all secure storage is the right thing to do
            SecureStorageProvider.RemoveAll();
        }

        public void ReScan(DateTimeOffset? timeToStartOn = null)
        {
            // FIXME this method dosn't work crashes on Scan.
            if (timeToStartOn == null)
                timeToStartOn = _Network.GetBIP39ActivationChainedBlock().Header.BlockTime;

            DateTimeOffset currentCreationTime = WalletManager.GetWallet().CreationTime;

            DestroyWallet();

            // Create wallet
            // FIXME: This should not be done like this.
            //        Wallet should be created but with data we already have on SecureStorageProvider for mnemonic and password.
            string guid = SecureStorageProvider.GetWalletId();
            string mnemonic = SecureStorageProvider.GetMnemonic();
            string password = SecureStorageProvider.GetPassword() ?? ""; // FIXME I fear this could be null.
            DateTimeOffset seedBirthday = SecureStorageProvider.HasSeedBirthday()
                ? DateTimeOffset.FromUnixTimeSeconds(SecureStorageProvider.GetSeedBirthday())
                : new DateTimeOffset(DateTime.UtcNow);
            string language = "english"; // FIXME get actual language
            int wordCount = 12; // FIXME get count from somehere else...

            Logger.Information("Unloaded wallet");
            WalletManager.UnloadWallet();

            Logger.Information("Creating wallet ({guid}) with password: {password}", guid, password);

            WalletManager.CreateWallet(
                guid,
                password,
                WalletManager.MnemonicFromString(mnemonic),
                language,
                wordCount,
                seedBirthday
            );

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

        public IEnumerable<TransactionData> GetAllAccountsTransactions()
        {
            if (WalletManager == null) return new List<TransactionData>();

            return WalletManager.GetAllAccountsByCoinType(CoinType.Bitcoin)
                .SelectMany((HdAccount account) => account.GetCombinedAddresses())
                .SelectMany((HdAddress address) => address.Transactions);
        }

        public IEnumerable<TransactionData> GetCurrentAccountTransactions()
        {
            if (CurrentAccount == null) return new List<TransactionData>();

            return GetAccountTransactions(CurrentAccount);
        }

        public IEnumerable<TransactionData> GetAccountTransactions(HdAccount account)
        {
            return account.GetCombinedAddresses().SelectMany(
                (HdAddress address) => address.Transactions
            );
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

        void AddNodesGroupEvents()
        {
            if (_NodesGroup is null) return;

            _NodesGroup.ConnectedNodes.Added += ConnectedNodes_Added;
            _NodesGroup.ConnectedNodes.Removed += ConnectedNodes_Removed;
        }

        void ConnectedNodes_Added(object sender, NodeEventArgs e)
        {
            var node = e.Node;

            Logger.Debug("Connected node added {0}", node.RemoteSocketAddress.ToString());

            ConnectedNodes++;
        }

        void ConnectedNodes_Removed(object sender, NodeEventArgs e)
        {
            var node = e.Node;

            Logger.Debug("Connected node removed {0}", node.RemoteSocketAddress.ToString());

            if (node.IsConnected)
            {
                node.Disconnected += (Node n) =>
                {
                    ConnectedNodes--;
                };
            }
        }

        PartialConcurrentChain GetChain()
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

        string GetConfigFile(string fileName)
        {
            string configFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

            Logger?.Information("Getting config file: {configFileName}", configFileName);

            return configFileName;
        }

        string AddrmanFile()
        {
            Guard.NotNull(_Network, nameof(_Network));

            return GetConfigFile($"addrman-{_Network.Name.ToLower()}.dat");
        }

        string ChainFile()
        {
            Guard.NotNull(_Network, nameof(_Network));

            return GetConfigFile($"chain-{_Network.Name.ToLower()}.dat");
        }

        async Task PeriodicSave()
        {
            while (true)
            {
                await Save();

                Logger.Information("Saved chain file to filepath: {filepath} on {now}", ChainFile(), DateTime.Now);

                Logger.Debug($"Chain file size: {new FileInfo(ChainFile()).Length}");

                int delay = 50_000;

                await Task.Delay(delay);
            }
        }

        async Task Save()
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

        AddressManager GetAddressManager()
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

        ChainedBlock GetClosestChainedBlockToDateTimeOffset(DateTimeOffset? creationDate)
        {
            Guard.NotNull(_Network, nameof(_Network));
            long ticks = 0;

            if (creationDate.HasValue)
                ticks = creationDate.Value.Ticks;

            return _Network.GetCheckpoints().OrderBy(chainedBlock => Math.Abs(chainedBlock.Header.BlockTime.Ticks - ticks)).FirstOrDefault();
        }
    }
}
