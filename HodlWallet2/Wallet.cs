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

            if (_Network == Network.Main)
            {
                // These are hodlwallet 1.0 checkpoints.
                //{ 0, 1, uint256("000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f"), 1231006505, 0x1d00ffff },
                //{ 20160, uint256("000000000f1aef56190aee63d33a373e6487132d522ff4cd98ccfc96566d461e"), 1248481816, 0x1d00ffff },
                //{ 40320, uint256("0000000045861e169b5a961b7034f8de9e98022e7a39100dde3ae3ea240d7245"), 1266191579, 0x1c654657 },
                //{ 60480, uint256("000000000632e22ce73ed38f46d5b408ff1cff2cc9e10daaf437dfd655153837"), 1276298786, 0x1c0eba64 },
                //{ 80640, uint256("0000000000307c80b87edf9f6a0697e2f01db67e518c8a4d6065d1d859a3a659"), 1284861847, 0x1b4766ed },
                //{ 100800, uint256("000000000000e383d43cc471c64a9a4a46794026989ef4ff9611d5acb704e47a"), 1294031411, 0x1b0404cb },
                //{ 120960, uint256("0000000000002c920cf7e4406b969ae9c807b5c4f271f490ca3de1b0770836fc"), 1304131980, 0x1b0098fa },
                //{ 141120, uint256("00000000000002d214e1af085eda0a780a8446698ab5c0128b6392e189886114"), 1313451894, 0x1a094a86 },
                //{ 161280, uint256("00000000000005911fe26209de7ff510a8306475b75ceffd434b68dc31943b99"), 1326047176, 0x1a0d69d7 },
                //{ 181440, uint256("00000000000000e527fc19df0992d58c12b98ef5a17544696bbba67812ef0e64"), 1337883029, 0x1a0a8b5f },
                //{ 201600, uint256("00000000000003a5e28bef30ad31f1f9be706e91ae9dda54179a95c9f9cd9ad0"), 1349226660, 0x1a057e08 },
                //{ 221760, uint256("00000000000000fc85dd77ea5ed6020f9e333589392560b40908d3264bd1f401"), 1361148470, 0x1a04985c },
                //{ 241920, uint256("00000000000000b79f259ad14635739aaf0cc48875874b6aeecc7308267b50fa"), 1371418654, 0x1a00de15 },
                //{ 262080, uint256("000000000000000aa77be1c33deac6b8d3b7b0757d02ce72fffddc768235d0e2"), 1381070552, 0x1916b0ca },
                //{ 282240, uint256("0000000000000000ef9ee7529607286669763763e0c46acfdefd8a2306de5ca8"), 1390570126, 0x1901f52c },
                //{ 302400, uint256("0000000000000000472132c4daaf358acaf461ff1c3e96577a74e5ebf91bb170"), 1400928750, 0x18692842 },
                //{ 322560, uint256("000000000000000002df2dd9d4fe0578392e519610e341dd09025469f101cfa1"), 1411680080, 0x181fb893 },
                //{ 342720, uint256("00000000000000000f9cfece8494800d3dcbf9583232825da640c8703bcd27e7"), 1423496415, 0x1818bb87 },
                //{ 362880, uint256("000000000000000014898b8e6538392702ffb9450f904c80ebf9d82b519a77d5"), 1435475246, 0x1816418e },
                //{ 383040, uint256("00000000000000000a974fa1a3f84055ad5ef0b2f96328bc96310ce83da801c9"), 1447236692, 0x1810b289 },
                //{ 403200, uint256("000000000000000000c4272a5c68b4f55e5af734e88ceab09abf73e9ac3b6d01"), 1458292068, 0x1806a4c3 },
                //{ 423360, uint256("000000000000000001630546cde8482cc183708f076a5e4d6f51cd24518e8f85"), 1470163842, 0x18057228 },
                //{ 443520, uint256("00000000000000000345d0c7890b2c81ab5139c6e83400e5bed00d23a1f8d239"), 1481765313, 0x18038b85 },
                //{ 463680, uint256("000000000000000000431a2f4619afe62357cd16589b638bb638f2992058d88e"), 1493259601, 0x18021b3e },
                //{ 483840, uint256("0000000000000000008e5d72027ef42ca050a0776b7184c96d0d4b300fa5da9e"), 1504704195, 0x1801310b },
                //{ 504000, uint256("0000000000000000006cd44d7a940c79f94c7c272d159ba19feb15891aa1ea54"), 1515827554, 0x177e578c },
                //{ 524160, uint256("00000000000000000009d1e9bee76d334347060c6a2985d6cbc5c22e48f14ed2"), 1527168053, 0x17415a49 },
                //{ 544320, uint256("0000000000000000000a5e9b5e4fbee51f3d53f31f40cd26b8e59ef86acb2ebd"), 1538639362, 0x1725c191 }
            }
            else
            {
                // HodlWallet 1.0 checkpoints.
                //{       0, uint256("000000000933ea01ad0ee984209779baaec3ced90fa3f408719526f8d77f4943"), 1296688602, 0x1d00ffff },
                //{  100800, uint256("0000000000a33112f86f3f7b0aa590cb4949b84c2d9c673e9e303257b3be9000"), 1376543922, 0x1c00d907 },
                //{  201600, uint256("0000000000376bb71314321c45de3015fe958543afcbada242a3b1b072498e38"), 1393813869, 0x1b602ac0 },
                //{  302400, uint256("0000000000001c93ebe0a7c33426e8edb9755505537ef9303a023f80be29d32d"), 1413766239, 0x1a33605e },
                //{  403200, uint256("0000000000ef8b05da54711e2106907737741ac0278d59f358303c71d500f3c4"), 1431821666, 0x1c02346c },
                //{  504000, uint256("0000000000005d105473c916cd9d16334f017368afea6bcee71629e0fcf2f4f5"), 1436951946, 0x1b00ab86 },
                //{  604800, uint256("00000000000008653c7e5c00c703c5a9d53b318837bb1b3586a3d060ce6fff2e"), 1447484641, 0x1a092a20 },
                //{  705600, uint256("00000000004ee3bc2e2dd06c31f2d7a9c3e471ec0251924f59f222e5e9c37e12"), 1455728685, 0x1c0ffff0 },
                //{  806400, uint256("0000000000000faf114ff29df6dbac969c6b4a3b407cd790d3a12742b50c2398"), 1462006183, 0x1a34e280 },
                //{  907200, uint256("0000000000166938e6f172a21fe69fe335e33565539e74bf74eeb00d2022c226"), 1469705562, 0x1c00ffff },
                //{ 1008000, uint256("000000000000390aca616746a9456a0d64c1bd73661fd60a51b5bf1c92bae5a0"), 1476926743, 0x1a52ccc0 },
                //{ 1108800, uint256("00000000000288d9a219419d0607fb67cc324d4b6d2945ca81eaa5e739fab81e"), 1490751239, 0x1b09ecf0 },
                //{ 1209600, uint256("0000000000000026b4692a26f1651bec8e9d4905640bd8e56056c9a9c53badf8"), 1507353706, 0x1973e180 },
                //{ 1310400, uint256("0000000000013b434bbe5668293c92ef26df6d6d4843228e8958f6a3d8101709"), 1527063804, 0x1b0ffff0 },
                //{ 1411200, uint256("00000000000000008b3baea0c3de24b9333c169e1543874f4202397f5b8502cb"), 1535560970, 0x194ac105 }
                checkpoints.Add((0, new BlockHeader(
                    "0100000000000000000000000000000000000000000000000000000000000000000000003ba3edfd7a7b12b27ac72c3e67768f617fc81bc3888a51323a9fb8aa4b1e5e4adae5494dffff001d1aa4ae180101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d0104455468652054696d65732030332f4a616e2f32303039204368616e63656c6c6f72206f6e206272696e6b206f66207365636f6e64206261696c6f757420666f722062616e6b73ffffffff0100f2052a01000000434104678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5fac00000000",
                    _Network.Consensus.ConsensusFactory
                    )));
                checkpoints.Add((100800, new BlockHeader(
                    "020000001939e922692d67e9da0c512082b3caaebaf04fac89499b07f310af000000000022c4fd8dd050b04bac685e24a0d0d6d21101ad605bc9effed2a654016e903836b2640c5207d9001cda8a7fb70401000000010000000000000000000000000000000000000000000000000000000000000000ffffffff3703c08901000407d9001c0417570000522cfabe6d6d0000000000000000000068692066726f6d20706f6f6c7365727665726aac1eeeed88ffffffff01a078072a010000001976a914912e2b234f941f30b18afbb4fa46171214bf66c888ac0000000001000000012bde870a76d5864149993d3379247e02df18639f54459d0b639e938cc579b5b6010000006a47304402200a0309dd1c592291942eebc10a44b5afe15acb0038bf0496f24a6d965ee3485b02206b1dad3c1191afbb717722490492f2f2214c9af6c2d21f664fffc2d4c00a5aac012102221b1343d47a4660a21d25bcc89361fde6f8624d1dd829560b9b4fe5e91a45a7ffffffff02e0834e02000000001976a9146953ce65058e5e68125a9163d74b277d6a7f4a9e88aca02b7208000000001976a91402613aca07110cd9dce0a1633f305ca8e5a8dc2e88ac000000000100000001fd8bc1898c487c3993fb2122d62a7aff551d4d14d6ad0cbe753ca635ec3accb4010000006b483045022100ae0278870da3300a89cbb653d2e05d7fe9ff17fd81b5276962f2278ef7731a3a022051be5081def0bfb5be719642028a042dbbe106607f318d7171664c49bab2fcfe0121035292bf8e7fe116c5acfbb6b6d2e546c6c0a7ff30d4cc4e87a0555fcf3aafdfa1ffffffff0290c04d02000000001976a9146953ce65058e5e68125a9163d74b277d6a7f4a9e88ace0e8cf02000000001976a91422170ca0a180d1cbefc4e45598aed3a2de05778088ac00000000010000000166405a0fbfaabfb855d83d93510f7321c92ff0b97ebeefcb81364b047ec8ffbb010000006c493046022100bca8d0963d3c01508a5f9bbf8093007f18fbb196227a34cbcec8c7e879018088022100e68ebd50155ddaa3359445928d7d6494e039bba4f78e926c10bd38b19d2b16ea012102221b1343d47a4660a21d25bcc89361fde6f8624d1dd829560b9b4fe5e91a45a7ffffffff0290c04d02000000001976a9146953ce65058e5e68125a9163d74b277d6a7f4a9e88ace0150104000000001976a91402613aca07110cd9dce0a1633f305ca8e5a8dc2e88ac00000000",
                    _Network.Consensus.ConsensusFactory
                    )));
                checkpoints.Add((201600, new BlockHeader(
                    "02000000ee689e4dcdc3c7dac591b98e1e4dc83aae03ff9fb9d469d704a64c0100000000bfffaded2a67821eb5729b362d613747e898d08d6c83b5704646c26c13146f4c6de91353c02a601b3a817f870401000000010000000000000000000000000000000000000000000000000000000000000000ffffffff0d038013030136062f503253482fffffffff013067062a010000002321023297b9a1516ea10f5706618528e9340ea83caa66d3049e93524e71ca459fb61cac000000000100000001971dd3c24299fe54d3b9f1f4c8d9c7a42122af97a0222b10159ec9f24d23d27f000000006a4730440220217e333362cfbbf6bcde7b444c29a25c71260ac6b94273354241da085d145a0a0220332381a2ca0c0b4e0ffacbab1724a72d34cfe3716c88cd3e2fd01e7ff96c59b9012103457f653b0ef1dd398fab1280d9200d5dec5917917c5866ab9743cb3d986d1280ffffffff02fd8d8600000000001976a914db9841b88b82f99de56539ead4d9f956c1d66e5588ac639ec44e000000001976a914f8c8ace1d6c7c8d3b590e2c78c8a8cc647b6b98788ac000000000100000001cd031a3d842959c1ae9dee16538fa032ff1772c9abd1f70d102754bfa08a7901010000008b48304502207c54aba14a5901654c861803e315dc942df7ec9d455302d9baa4e103b0eb0b37022100ba5af4f3af077fa7d11391b8b4529dcb66454061e6761c14256061493635a05d014104e1934263e84e202ebffca95246b63c18c07cd369c4f02de76dbd1db89e6255dacb3ab1895af0422e24e1d1099e80f01b899cfcdf9b947575352dbc1af57466b5ffffffff02a8610000000000001976a9149f339d94501dac3d0bc16e9e810be13fa9a1ae5688ace0344603000000001976a914cf0dfe6e0fa6ea5dda32c58ff699071b672e1faf88ac0000000001000000014d58df502e2fab837c5b69a1facf1a7e97502c353d95a100a9d35bb252ebb6e3010000008c493046022100ab5e8d5db76ed3c1d73598c6adfb123666093ca10e03120a5eaa2eb9e0ec573e022100d50142bee5cf569c050375296bfd577fd9393087d02eb78a7f3b0993a0414615014104e1934263e84e202ebffca95246b63c18c07cd369c4f02de76dbd1db89e6255dacb3ab1895af0422e24e1d1099e80f01b899cfcdf9b947575352dbc1af57466b5ffffffff02a8610000000000001976a9149f339d94501dac3d0bc16e9e810be13fa9a1ae5688ac48411228000000001976a914cf0dfe6e0fa6ea5dda32c58ff699071b672e1faf88ac00000000",
                    _Network.Consensus.ConsensusFactory
                    )));
                checkpoints.Add((302400, new BlockHeader(
                    "02000000a5895a55e1291fc575f21f107adfb24f4adfba8a75deb716ed32000000000000c6cd6732a04c51f08b2af9ed3277ddf83f5cb97cf6e90b30dda26f1aa2f575245f5c44545e60331a024203fb0c01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2603409d04194d696e656420627920416e74506f6f6c200000000054445c5f0000000037310300ffffffff01101a0495000000001976a9145ac88acaa618074e8271420a5a9faa28831b073e88ac00000000010000000167064e1a3cf8d896efade33431dfd5938812856d915c37815729822ac305498101000000d900473044022038b09d595ffc111b82fc039976f85f3b216f3116575fc1143b9cbbea0935bfea02205823e2398d4e836f0fc418e79324a76bbe3d87c6b07e4f1dfc82e943b90ef6d601473044022053bfd16a87929a3b7718e02eaee947e5a7c39f7696c8f8a7f5d3f46a87f5ae2002206531586721f649a130b2bc6312bf85e9c0cf8cef2d20a2bda60f078beb7b35b90147522102632178d046673c9729d828cfee388e121f497707f810c131e0d3fc0fe0bd66d62103a0951ec7d3a9da9de171617026442fcd30f34d66100fab539853b43f508787d452aeffffffff0240420f000000000017a914e69d1cc3818e2c795fed9185ff92a31adfe9c1c9876014e0270100000017a9148ce5408cfeaddb7ccb2545ded41ef47810945484870000000001000000018aadc496130f2ddc77e9224244915932964ec5b2787cb6559fad5d30021ed0a401000000da00473044022062a7550b3d7f16525c036965468b4437dc22b1c1dc668ede85b8049664c73ca302204c29aacdc07834af9d71e325e33957691cb26440807e58ecaf69e3aadba5f6e901483045022100c4275cb10f8b1624dabf51ef9ac84ee38c57a927b3b11dd38d8c2334343e03d802207967e7b06ed703440e1b3fe492043bcfe473026738fddaa9c89fb6088fa3016f0147522102632178d046673c9729d828cfee388e121f497707f810c131e0d3fc0fe0bd66d62103a0951ec7d3a9da9de171617026442fcd30f34d66100fab539853b43f508787d452aeffffffff0240420f000000000017a91461f8963394aa095d78454c95b04eb715b088379487885aef270100000017a9148ce5408cfeaddb7ccb2545ded41ef47810945484870000000001000000013ddaf4a5b4a67080b1e376e10dc5f4cbe6070b9caaecc37624b99945d871b58601000000da004830450221009c4e0cbda1e1e963d4e2a192c81d1a547908f3d3fa3927934719f1678ecc5b5102205c5a6d7c6f11826317b696f781bcc28d49578d5ac158084d8da9d0a045089f260147304402205a26eae9cb1bcb3345af4f9fec853c866cd44cb74683337774dd36a11540835c022002da22d45efe4bc6972cbe4fa7ab2255a2d85d5a8c824defb8a49f660a65fecf0147522102632178d046673c9729d828cfee388e121f497707f810c131e0d3fc0fe0bd66d62103a0951ec7d3a9da9de171617026442fcd30f34d66100fab539853b43f508787d452aeffffffff0240420f000000000017a914f83989980a10d18f3f78d0587e033e5f10bada9f876014e0270100000017a9148ce5408cfeaddb7ccb2545ded41ef47810945484870000000001000000014588f2acc3dd8e648ec7f1eaf3fdd9da543c97fc01ef6c7fa2e7710577fbe4d501000000db00483045022100fa6a67b1080091f345da4d25a95e9726de4f7a21fd4da1a1b19a4960b181d23802205698c09ff313152dbed855476628a0eb015ae6a3a81fc242c13920cc9ffc11d801483045022100c0245564f220a7ee27cf1773ae0e47064fe53185d2b101d6793356bcf7ed2d790220065485d21f61bde0d956f9650ab3f845399d9f68b2250d86a988452d7fea7cca0147522102632178d046673c9729d828cfee388e121f497707f810c131e0d3fc0fe0bd66d62103a0951ec7d3a9da9de171617026442fcd30f34d66100fab539853b43f508787d452aeffffffff0240420f000000000017a914f0b8a53bd8e14fc90fcfc2b5f3a65ab874ff849c876014e0270100000017a9148ce5408cfeaddb7ccb2545ded41ef47810945484870000000001000000037c38a228cad6bcc8e1b982f852fee2a9dd67add9f71714adb382c5b08c8cfd9a010000006a47304402207cdca48387b9bb81707baa111a9d631cbf3100913846e58245d83ec5a230ef8d02202cef1c63ed275f35dcb3e8c78c788aeda74fd9dae2ed87b04b590a5be05880f2012103bbe17a7e58c6a13849e49060de6573a058a2644d31605bee5ec5e3a58cfadbb2ffffffffa425e077f49c8bc32578f3c2cc7cf178037dbebddce12dffc74a733541e6342c000000006b483045022100df44061b7c7168e9fc082fb783e2d4b3dc73ebe29d9e3476be3ccee361dcf2ab02205d0d03c28bc637adc1b2384b59ffcb4d9d48c49694c22f95c3cbc8172d2abddf012103bbe17a7e58c6a13849e49060de6573a058a2644d31605bee5ec5e3a58cfadbb2ffffffff3de674160f5ada424b3af8a4576527d9b5cf1e9ba4b3996b6f9e040730f4eb7b010000006a473044022011b2eb28d99e2bbf33ee1c1a60642d8bff49ab3bd85d9cceec66d96a92657c9702204366e43160b007c9222bf4efeabc131eb8d76c30e779a69114dea516182a5f39012102bb2928ce122e77be44356e4da5f20f871c93a5912e2b0c06932359b7caa248b5ffffffff02f0b18b09000000001976a914cf8f8db43882c3f1748da6c8bd9894b920de898d88ac00e1f505000000001976a914a8fb971c38fad5be550cccda78a21af83992219088ac00000000010000000126473106f7bd1c49520b3364b360e04eef3aac28bf9706e23152b8edfea71ae5010000008b4830450220596810ace48b8a76cec11be98cb026ba2608a230842f8a7ae350a882cbb651c50221008a1b668f8964224dee7972c6911d914dab5454cd10fcd79609cf8c758dda21780141040cfa3dfb357bdff37c8748c7771e173453da5d7caa32972ab2f5c888fff5bbaeb5fc812b473bf808206930fade81ef4e373e60039886b51022ce68902d96ef70ffffffff02a0860100000000001976a9140d4fd06b6d41424a6e1988d3d3f0ce55c2c275e388acb4d2aa700a0000001976a91461b469ada61f37c620010912a9d5d56646015f1688ac000000000100000001cc67fe4db9f869e4905f37bd2843b55e218e68815e686e52b9549b39dabbcd34010000006b483045022100b418ad11d95811e44770a47457a27a566349fa21e33f7e774213cfa3b72ad67202205244b1b6f5d76dfe83fd8464b27303efa5aac3e931c0a747e079b4369d640f1a012103d47a0d6b0ee5eed95ce45ce190d251ea580c23dada28ea441482b2f9a04c4450ffffffff0240420f00000000001976a91433a48277fd7ea7a143f1d7dfca07cb2a9eb5900688ac19192401000000001976a91499f558d72346945a83c2dbd629c949dc5ccf6d0088ac0000000001000000014a557780571e435bf44c056b313536ce0a192a2269fb765bf3f07c983cb17d03010000008c493046022100a38f718394d7e0c9a327dbae3612d9b0000a8a51381b366b30ce052ad520950c022100cc89035efac03c315b9c8a8124f867a8afec8ffec7eb0f11670d5a78c33813a90141040cfa3dfb357bdff37c8748c7771e173453da5d7caa32972ab2f5c888fff5bbaeb5fc812b473bf808206930fade81ef4e373e60039886b51022ce68902d96ef70ffffffff02a0860100000000001976a914a566a3294c2df9b208c43dd814edb0698b61d8a288ac0425a9700a0000001976a91461b469ada61f37c620010912a9d5d56646015f1688ac0000000001000000024ecd377c2e218001f67132733b6594f1ebffdbf62c5101242b84988ddca54c34000000006a47304402203e11c63c040e63f0d15ef5276772e1ce32c652dcdebfa94762cad0969cb9699502207445678051b5b05f35b10fa84bca7da05fdee1b590673053c1e1c9ef8abb92920121023810c5e828aa965aff2b9b24c7a7ecaca119ccb0211273ef4dd9c81d61270c18ffffffffa8702bed6199da202b5646ab64b94b6ef536e608b1ab9e9a47f583188913992e010000006a473044022062053d15728ccbfec301ed42f79e633e21ef559fffbbe8391cab835d1bfdcb84022034831fd3364b06a3191296f320a91501be94839927c6a1e6459f45f655cb3a190121022a0a4f85dde592a30b27c5765fb99bd78a5ee17fdbdde1d054cb0d0f477b070effffffff0210af1300000000001976a91446f3e3817763521630716997a08877025d10a2c488ac40420f00000000001976a914afc71fc02f0da528c21bb0b2717e26329849205888ac0000000001000000037399388ba8c748a37970670af54d804c0050873fcc281404f4132305fec9716a010000006b483045022100ddba04cd3cf9813fdd868599f3dd73f599560ffc6a546186c5bc446088e0af2b022078030085bf8d10578d000d05dce27c9b51d288dcea6cab0eaba2583dffdb5db50121021be6e6a6cf7baeefd3185bb946a243edfd95828f8140303ef1c0e420f5a80646ffffffffdd7f1c6f58065ecd81a07085a60ffa192078461d94ba58cfc3c665bd42e3b728010000006a47304402200f29e6020c26e62e30d5c02efd3f0025a20c7d859b3623744c76c6a2076b30480220557fd76b0e1e11ed03da3bc3df13d7bf8ecb0c2c8b75ec6d60fc6ed653a80a2c012102631303b04dc9f926c06dfa8a913d77ab6d6be23620519998bac6cfca572ef993fffffffffa7f405e270ac86aa3015f42d2c1ff3d3c16a8b31886db53046d5e8ce3ffaee4000000006b483045022100b0b4b5edb4374c48a7852b705831e8b867c8159269c93ab004f2c8bcfc40caa902207603e1942dc1e5c0972284dfe6ae162c3b1b733d1bf5817a8e9bc6acf5a8f7980121024b734fb8b9e5477ee21491198ed95f8c1d357682ef883d7f833f9e42717c8a25ffffffff02a0f01900000000001976a914b6ee912dd12bca24a6d599bd4e635034126e7d9988ac10af1300000000001976a9146de28efe5ca6184d6d0680ea5ceebae9a101842c88ac0000000001000000030f1e9dbe028fc9cee006e10dc987996e0a3066d572c52f4fb78c62eb2a5d12a0000000006b483045022100acac88e294dea4ddf9b1fecb02ccac0fc376925cb5915c4c98272a0d1c98f23a02201b14a8357fbebd30fd65c30b4447ace856f25677415e1e0ae981cab6d1c1d825012102960f199338ef8247f840e9390c2511100ebe789f4f43b78b4382b8c24dbc70ffffffffff8011869abdc072ba3cbfd253fa435314a11072d896f7445e333bad68c4e80002010000006b483045022100f251cb94ee13a61c4bb2ff8f37f58f5ee9f831a77785c20d09151c86a5f0dfab022038e707778cc0c65ebd379c3c18ce4ab7f5ce9614df91307cc98f10fc8bb0591a012102eca2c473d4cb917d1c1ccbd794b0de1ce25b105baf4583cdf3de2abdf08cec58ffffffffcea1e1415915a2e6f1be85a20ffd2e30ad9555babc183c06f094d84a5e50279b010000006b483045022100ccabc302a394d8c91954c1bce96c6c96f7c714298d0a7dc9f7ac7eb69340393202203aeb14bb802975e24f10f9cd69d36c6430063dc5b92e867c05d181e71024c18b012103e2c64dadf0e1f446c1a9f80c1f400d346a00a47c2292507e61c34688fd5425a8ffffffff0240420f00000000001976a9146eb44724f47c21aa359b194c6d12a943d4586db288ac705d1e00000000001976a914f89db445582db61532bf1cf128921232f5b73a7688ac00000000",
                    _Network.Consensus.ConsensusFactory
                    )));
                checkpoints.Add((403200, new BlockHeader(
                    "030000007ccf123dcf34b0c5627969c6eebfe4934fbd65244bf93109cd5282000000000065eecfb9cb61b373d7e8da19fa27ebab09c3a367369506763f173a5e40f8b64062dd57556c34021c218f078c0101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff230300270607062f503253482f030000006266676d696e657220352e312e30001b000000ffffffff0100f90295000000001976a91449776a3be69fa71e2d12d14d0d7d696c28fd395e88ac00000000",
                    _Network.Consensus.ConsensusFactory
                    )));
                checkpoints.Add((504000, new BlockHeader(
                    "0300000069ea2a30f71b993f5692ed2d7feb266d8780d7a327743e79e6af010000000000619d9580aef06044392706715e917766e71d5a594b0299d08949209f563d4c4f8a25a65586ab001b831c31290201000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703c0b007062f503253482f046d14a65508f8000001813062000d2f7374726174756d506f6f6c2f000000000190a3814a000000001976a914dea0cc5e20099e5a2ad5f774e21e9abc8cde3a7988ac00000000010000000161711ee0599e4a851336804ecf103d75ab5161df3d0bd9256d3a7ba2345b2ebb000000006a47304402205cf006644b71f0a706934ca0b8b767ffa3920130d94b3e829564112d9a0bdd9e02205436f86338852bcfc98677c848324e13b1e5288b1151f98519ff08ce0f11746101210327cba90a9e8b37ff5e77064c59065ffdf46a6f7cf801e7d41a245a0169aaf6aaffffffff0500000000000000002a6a28436875636b204e6f72726973207175616c69666965642077697468206120746f702073706565642000000000000000002a6a286f6620333234206d70682061742074686520446179746f6e61203530302c20776974686f757420610000000000000000076a05206361722e10270000000000001976a914fff1afa15aff10e6560c3b290b763f450771a51c88ac22870000000000001976a91432df940559ff7263ca8dba3b414be4a682b3b5cf88ac00000000",
                    _Network.Consensus.ConsensusFactory
                    )));
                //checkpoints.Add((604800, new BlockHeader(
                    //"",
                    //_Network.Consensus.ConsensusFactory
                    //)));
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
                foreach (var checkpoint in GetCheckpoints())
                {
                    if (checkpoint.Height > chain.Height)
                        chain.SetTip(checkpoint.BlockHeader);
                }

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

                int delay = 50_000;
                if (!Instance._Chain.IsDownloaded())
                    delay = 10;

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
