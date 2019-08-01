using System;
using System.Collections.Generic;
using System.Security;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

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

using HodlWallet2.Core.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(WalletService))]
namespace HodlWallet2.Core.Services
{
    public sealed class WalletService : IWalletService
    {
        public static WalletService Instance => DependencyService.Get<WalletService>() ?? new WalletService();

        public const int DEFAULT_NODES_TO_CONNECT = 4;

        public const string DEFAULT_NETWORK = "testnet";

        public static string USER_AGENT { get; } = $"{Liviano.Version.UserAgent}/hodlwallet:2.0/";

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
        public event EventHandler OnScanningFinished;

        public bool IsStarted { get; private set; }
        public bool IsConfigured { get; private set; }

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
                return WalletManager.Wallet.GetAccountsByCoinType(CoinType.Bitcoin).FirstOrDefault();
            }

            set => throw new NotImplementedException("This should switch the current account.");
        }

        public void InitializeWallet()
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

            Configure(walletId: guid, network: network);

            if (SecureStorageService.HasMnemonic() && _WalletId != null)
            {
                StartWalletWithWalletId();

                SecureStorageService.SetSeedBirthday(
                    WalletManager.Wallet.CreationTime
                );

                Logger.Information("Since wallet has a mnemonic, then start the wallet.");

                return;
            }

            Logger.Information("Wallet has been configured but not started yet due to the lack of mnemonic in the system");
        }

        public void StartWalletWithWalletId()
        {
            Guard.NotNull(_WalletId, nameof(_WalletId));

            string mnemonic = SecureStorageService.GetMnemonic();
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
                    if (!HdOperations.IsMnemonicOfWallet(mnemonic, WalletManager.Wallet))
                    {
                        WalletManager.GetStorageProvider().DeleteWallet();

                        string language = "english";
                        int wordCount = 12;
                        DateTimeOffset createdAt = SecureStorageService.HasSeedBirthday()
                            ? DateTimeOffset.FromUnixTimeSeconds(SecureStorageService.GetSeedBirthday())
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
                DateTimeOffset createdAt = SecureStorageService.HasSeedBirthday()
                    ? DateTimeOffset.FromUnixTimeSeconds(SecureStorageService.GetSeedBirthday())
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
            Start(password, WalletManager.Wallet.CreationTime);

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

            _ConParams.UserAgent = USER_AGENT;

            _NodesGroup = new NodesGroup(_Network, _ConParams, new NodeRequirement()
            {
                RequiredServices = NodeServices.Network
            });

            Logger.Information("Requiring service 'NETWORK' for SPV");

            BroadcastManager = new BroadcastManager(_NodesGroup);

            _ConParams.TemplateBehaviors.Add(new TransactionBroadcastBehavior(BroadcastManager));

            _NodesGroup.NodeConnectionParameters = _ConParams;
            _NodesGroup.MaximumNodeConnection = _NodesToConnect;

            _DefaultCoinSelector = new DefaultCoinSelector();

            Logger.Information("Coin selector: {coinSelector}", _DefaultCoinSelector.GetType().ToString());

            TransactionManager = new TransactionManager(BroadcastManager, WalletManager, _DefaultCoinSelector, _Chain);

            Logger.Information("Add transaction manager.");

            Logger.Information("Configured wallet.");

            OnConfigured?.Invoke(this, null);
            IsConfigured = true;
        }

        public void Start(string password, DateTimeOffset? timeToStartOn = null)
        {
            if (WalletManager.Wallet == null)
            {
                WalletManager.LoadWallet(password);

                timeToStartOn = WalletManager.Wallet.CreationTime;
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
                string networkStr = SecureStorageService.GetNetwork();

                _Network = Network.GetNetwork(networkStr);
            }

            if (StorageProvider == null)
            {
                string walletId = SecureStorageService.GetWalletId();

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
            SecureStorageService.RemoveAll();
        }

        public void ReScan(DateTimeOffset? timeToStartOn = null)
        {
            if (timeToStartOn == null)
                timeToStartOn = _Network.GetBIP39ActivationChainedBlock().Header.BlockTime;

            if (SecureStorageService.HasSeedBirthday())
                timeToStartOn = DateTimeOffset.FromUnixTimeSeconds(SecureStorageService.GetSeedBirthday());

            // NOTE Should rescan handle deletion of tx database from the wallet? I don't think so	

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

        public (bool Success, Transaction Tx, decimal Fees, string Error) CreateTransaction(decimal amount, string addressTo, int feeSatsPerKB, string password)
        {
            Money btcAmount = new Money(amount, MoneyUnit.BTC);
            Transaction tx = null;
            decimal fees = 0.0m;

            try
            {
                tx = TransactionManager.CreateTransaction(
                    addressTo,
                    btcAmount,
                    feeSatsPerKB,
                    CurrentAccount,
                    password,
                    signTransaction: true
                );
                fees = tx.GetVirtualSize() * (feeSatsPerKB / 1000);

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

        public Network GetNetwork()
        {
            Guard.NotNull(WalletManager, nameof(WalletManager));

            return WalletManager.Network;
        }

        /// <summary>
        /// Check if synced to tip, this method will check a few things and return
        /// false, otherwise it returns true as we assume the blockchain is synced
        /// </summary>
        /// <returns>A <see cref="bool"/> true if the chain is synced</returns>
        public bool IsSyncedToTip()
        {
            var chainTip = _Chain.Tip;

            // We cannot be sure if we're synced to chain if we're not connected
            if (_ConnectedNodes < 1) return false;

            // Tip is null so we get out
            if (chainTip is null) return false;

            // The headers are behind the activation block
            if (chainTip.Height < _Network.GetBIP39ActivationChainedBlock().Height) return false;

            // The headers are behind the last checkpoint
            if (chainTip.Height < _Network.GetCheckpoints().Last().Height) return false;

            // TODO Sometimes, the next condition will be true, this is due us still syncing the headers
            // the hard thing about this problem is knowing what's the tip of the chain before getting all
            // the headers.

            // What assumptions can I make?

            // If the block time is less than 30 mins before now then it's likely okay
            long seconds = DateTimeOffset.Now.ToUnixTimeSeconds() - chainTip.Header.BlockTime.ToUnixTimeSeconds();
            long minutes = seconds / 60;

            if (minutes > 30) return false;

            // We cannot allow the last syncing tip to be different than the last received block hash
            if (chainTip.HashBlock != WalletManager.LastReceivedBlockHash()) return false;

            // We're synced to our last known chain tip from get headers...
            return true;
        }

        public string GetLastSyncedDate()
        {
            var syncedTip = _Chain.FindFork(
                new uint256[] {
                    WalletManager.LastReceivedBlockHash()
                }
            );

            if (syncedTip != null)
                return syncedTip.Header.BlockTime.ToString("dddd, dd MMMM yyyy");

            if (WalletManager.GetWalletBlockLocator() != null)
                syncedTip = _Chain.FindFork(WalletManager.GetWalletBlockLocator());

            if (syncedTip != null)
                return syncedTip.Header.BlockTime.ToString("dddd, dd MMMM yyyy");

            // We can only asume on network's bip 39 activation block.
            var blockTime = _Network.GetBIP39ActivationChainedBlock().Header.BlockTime;
            return blockTime.ToString("dddd, dd MMMM yyyy");
        }

        public string GetSyncedProgressPercentage()
        {
            return (GetSyncedProgress() * 100).ToString("0.##");
        }

        public double GetSyncedProgress()
        {
            var tip = _Chain.FindFork(new uint256[] { WalletManager.LastReceivedBlockHash() });
            var minutesPerBlock = _Network == Network.Main ? 10 : 8; // Based on stimates and calculations

            if (tip is null) tip = _Chain.FindFork(WalletManager.GetWalletBlockLocator() ?? new uint256[] { });
            if (tip is null) tip = _Network.GetBIP39ActivationChainedBlock();

            // Time aproximates... Based on probability of a bitcoin block being bethween 10 minutes
            // In my experience I think 15 is fine.

            // If we are close to it, then we say we're 1.00 synced
            long seconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - tip.Header.BlockTime.ToUnixTimeSeconds();
            int minutes = (int)(seconds / 60);

            if (minutes <= 15) return 1;
            if (minutes <= 30) return .99;

            int aproximateBlocksBehind = minutes / minutesPerBlock;
            int currentBlockHeight = GetLastSyncedBlockHeight();
            int bip39ActivationBlockHeight = _Network.GetBIP39ActivationChainedBlock().Height;

            int predictedBlockHeight = currentBlockHeight + aproximateBlocksBehind;

            double progress = (double)(currentBlockHeight - bip39ActivationBlockHeight) / (double)predictedBlockHeight;

            Logger.Debug("[{methodName}] Progress: {progress}", nameof(GetSyncedProgress), progress);

            return progress;
        }

        public long GetCurrentAccountBalanceInSatoshis(bool includeUnconfirmed = false)
        {
            var balance = WalletManager.GetBalances(CurrentAccount.Name).FirstOrDefault();

            if (includeUnconfirmed)
                return balance.AmountConfirmed + balance.AmountUnconfirmed;

            return balance.AmountConfirmed;
        }

        public decimal GetCurrentAccountBalanceInBTC(bool includeUnconfirmed = false)
        {
            long sats = GetCurrentAccountBalanceInSatoshis(includeUnconfirmed);
            decimal satsPerBtc = 100_000_000m;

            return decimal.Divide(new decimal(sats), satsPerBtc);
        }

        public int GetLastSyncedBlockHeight()
        {
            var tip = _Chain.FindFork(new uint256[] { WalletManager.LastReceivedBlockHash() });

            if (tip is null) tip = _Chain.FindFork(WalletManager.GetWalletBlockLocator() ?? new uint256[] { });

            if (tip is null) tip = _Network.GetBIP39ActivationChainedBlock();

            return tip.Height;
        }

        public ChainedBlock GetChainTip()
        {
            return _Chain.Tip;
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

            ConnectedNodes = _NodesGroup.ConnectedNodes.Count;
        }

        void ConnectedNodes_Removed(object sender, NodeEventArgs e)
        {
            var node = e.Node;

            Logger.Debug("Connected node removed {0}", node.RemoteSocketAddress.ToString());

            if (node.IsConnected)
            {
                node.Disconnected += (Node n) =>
                {
                    ConnectedNodes = _NodesGroup.ConnectedNodes.Count;
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
