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

        private IEnumerable<(int Height, BlockHeader BlockHeader)> GetCheckpoints()
        {
            List<(int Height, BlockHeader BlockHeader)> checkpoints = new List<(int, BlockHeader)> ();
            List<(int Height, int Version, uint256 PrevBlockHeaderHash, uint256 MerkleRootHash, uint Time, uint NBits, uint Nonce)> rawBlockHeaders = new List<(int, int, uint256, uint256, uint, uint, uint)>();

//            if (_Network == Network.Main)
//            {
//            }
//            else
//            {
//                rawBlockHeaders.Add(
//(100800, 2, new uint256("0000000000af10f3079b4989ac4ff0baaecab38220510cdae9672d6922e93919"), new uint256("0000000000a33112f86f3f7b0aa590cb4949b84c2d9c673e9e303257b3be9000"), uint.Parse("1376543922"), uint.Parse("469817607"), uint.Parse("3078589146"))
//);
            //}

            foreach (var rawBlockHeader in rawBlockHeaders)
            {
                var blockHeader = _Network.Consensus.ConsensusFactory.CreateBlockHeader();
                var bitcoinStream = new BitcoinStream(new MemoryStream(), serializing: true);

                bitcoinStream.ReadWrite(rawBlockHeader.Version);
                bitcoinStream.ReadWrite(rawBlockHeader.PrevBlockHeaderHash);
                bitcoinStream.ReadWrite(rawBlockHeader.MerkleRootHash);
                bitcoinStream.ReadWrite(rawBlockHeader.Time);
                bitcoinStream.ReadWrite(rawBlockHeader.Nonce);

                blockHeader.ReadWrite(bitcoinStream);

                //blockHeader.Version = rawBlockHeader.Version;
                //blockHeader.HashPrevBlock = rawBlockHeader.PrevBlockHeaderHash;
                //blockHeader.HashMerkleRoot = rawBlockHeader.MerkleRootHash;
                //blockHeader.BlockTime = DateTimeOffset.Parse(rawBlockHeader.Time.ToString());
                //blockHeader.Bits = new Target(rawBlockHeader.NBits);
                //blockHeader.Nonce = rawBlockHeader.Nonce;

                checkpoints.Add((rawBlockHeader.Height, blockHeader));
                //var blockHeader = new BlockHeader(bitcoinStream., _Network);
            }

            return checkpoints;
        }

        private ConcurrentChain GetChain()
        {
            lock (_Lock)
            {
                var chain = new ConcurrentChain(_Network);

                if (_ConParams != null)
                {
                    chain = _ConParams.TemplateBehaviors.Find<ChainBehavior>().Chain;
                }
                else
                {
                    using (var fs = File.Open(ChainFile(), FileMode.OpenOrCreate))
                    {
                        chain.Load(fs);
                    }
                }

                // Add default checkpoints if our chain tip is not up to our checkpoints lastest header
                //foreach (var checkpoint in GetCheckpoints())
                //{
                    //if (checkpoint.Height > chain.Height)
                        //chain.re
                        //chain.SetTip(new ChainedBlock(checkpoint.BlockHeader, checkpoint.Height));
                //}

                return chain;
            }
        }

        private static string GetConfigFile(string fileName)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

            return filePath;
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

                await Task.Delay(50_000);
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
            StorageProvider = new HodlWallet2StorageProvider(_WalletId);

            if (!StorageProvider.WalletExists())
            {
                Logger.Information("Creating a new wallet {walletId}", _WalletId);
            }

            WalletManager = new WalletManager(Logger, _Network, _Chain, AsyncLoopFactory, DateTimeProvider, ScriptAddressReader, StorageProvider);
            WalletSyncManager = new WalletSyncManager(Logger, WalletManager, _Chain);

            _WalletSyncManagerBehavior = new WalletSyncManagerBehavior(Logger, WalletSyncManager, ScriptTypes.SegwitAndLegacy);

            _ConParams.TemplateBehaviors.Add(new AddressManagerBehavior(_AddressManager));
            _ConParams.TemplateBehaviors.Add(new ChainBehavior(_Chain));
            _ConParams.TemplateBehaviors.Add(_WalletSyncManagerBehavior);

            _ConParams.UserAgent = "hodlwallet:2.0";

            _NodesGroup = new NodesGroup(_Network, _ConParams, new NodeRequirement()
            {
                RequiredServices = NodeServices.Network
            });

            Logger.Information("Requiring service 'NETWORK' for SPV");

            BroadcastManager broadcastManager = new BroadcastManager(_NodesGroup);

            _ConParams.TemplateBehaviors.Add(new TransactionBroadcastBehavior(broadcastManager));

            _NodesGroup.NodeConnectionParameters = _ConParams;
            _NodesGroup.MaximumNodeConnection = _NodesToConnect;

            ScanLocation = new BlockLocator();
            ScanLocation.Blocks.Add(_Network.GenesisHash);

            Logger.Information("Adding Genesis block ({hash}) to blockchain scanner", _Network.GenesisHash.ToString());

            _DefaultCoinSelector = new DefaultCoinSelector();

            Logger.Information("Coin selector: {coinSelector}", _DefaultCoinSelector.GetType().ToString());

            Logger.Information("Configured wallet.");
        }

        public void Start(string password, DateTimeOffset? timeToStartOn = null)
        {
            WalletManager.LoadWallet(password);

            _NodesGroup.Connect();

            WalletManager.Start();

            Scan(timeToStartOn);

            PeriodicSave();
        }

        public void Scan(DateTimeOffset? timeToStartOn)
        {
            ICollection<uint256> walletBlockLocator = WalletManager.GetWalletBlockLocator();
            if (walletBlockLocator != null)
            {
                ScanLocation.Blocks.AddRange(walletBlockLocator);
            }
            else
            {
                ScanLocation.Blocks.Add(_Network.GenesisHash);
            }

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

        public TransactionData[] GetCurrentAccountTransactions()
        {
            var txs = WalletManager
                .GetAllAccountsByCoinType(CoinType.Bitcoin)
                .SelectMany((HdAccount arg) => arg.GetCombinedAddresses())
                .SelectMany((HdAddress arg) => arg.Transactions);

            return txs.ToArray();
        }
    }

}
