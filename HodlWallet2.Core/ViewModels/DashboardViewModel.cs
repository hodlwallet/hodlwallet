using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.Utils;
using Liviano.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using Newtonsoft.Json;

namespace HodlWallet2.Core.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IWalletService _walletService;

        public string SendText => "Send";
        public string ReceiveText => "Receive";

        private ObservableCollection<Transaction> _transactions;

        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
        }

        public MvxCommand NavigateToSendViewCommand { get; private set; }
        public MvxCommand NavigateToReceiveViewCommand { get; private set; }
        public MvxCommand NavigateToMenuViewCommand { get; private set; }
        
        public DashboardViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) : base(logProvider, navigationService)
        {
            _walletService = walletService;
            NavigateToSendViewCommand = new MvxCommand(NavigateToSendView);
            NavigateToReceiveViewCommand = new MvxCommand(NavigateToReceiveView);
            NavigateToMenuViewCommand = new MvxCommand(NavigateToMenuView);

            TransactionData first = JsonConvert.DeserializeObject<TransactionData>(
                "{\"id\": \"b1e636e66ac0590cab93e96144267895f10916c51591f3507c260ab661cb6f0b\"," +
                "\"amount\": 23507," +
                "\"isCoinStake\": false," +
                "\"index\": 0," +
                "\"creationTime\": \"1558040376\"," +
                "\"merkleProof\": \"480000000d2b2b4b4b5359f89200c3ed8a5a21445e5b8bbc7fb8c72b0f2b3049fbd1dbb2be5a54a8ed0efe6be556dd382327c146963aa6761f67e07ef78654a6e7f9129fd2e557d8c77dc1ea05cb0a0e3b3d8b3a3035a0066bc231916681a38e47468200a5e989f8f38b658fbaec9a427eb80bdc1a15a508205b5fe2e2b403eeb8418bbff14484fb38532d9f51fe19d93fcea8d64abb893c403e3aa2cae9edb4e43e8ff8150b6fcb61b60a267c50f39115c51609f19578264461e993ab0c59c06ae636e6b115a7afaad73f5257595a85beac414e854bba087f4338d40f1275f1d1000e47be144c558a8eb7c8c775b63b2c6f326627928a96fb122d55377186bda5846991d66cf80cf18b74037058ac16063d4ffb28a49d2b56012892c9284a9bd126aed13ec55b829b0794f547d53b20fb35de2030e5e5a3a9d94c901ae274b6fc3b6d3625b806ba7cf622d11997657b0ab8d3bc2c682476c13b236c8733e890d2083f0011699c64914415783035f0794b2cc00594550c94b35884b63271e8fe96ec9443745a118cd38e48c98174bae0f51351d76b23c35b3181cf13e4a8b8f1afa68258cb04afff0f00\"," +
                "\"scriptPubKey\": \"76a91464f98576035cb19a3e68d1b2fab0b1839f59946b88ac\"," +
                "\"hex\": \"0100000001e989f8f38b658fbaec9a427eb80bdc1a15a508205b5fe2e2b403eeb8418bbff1010000006b4830450221008978b7ef4a45630765c2039507735e9b159aee53f828d01831036ed1940bfd1602203d3d2363d1ee3f3401d7feed886e20961863e3b88df15ce963c282658c41c188012103f809fdf01ed2da0e3242ec0f25fe585f31b32da4decc062a961680a8235dcc2bffffffff02d35b0000000000001976a91464f98576035cb19a3e68d1b2fab0b1839f59946b88ac3ddd0000000000001976a9147bf2a7c5e6ca4c4a4871104b21402666c2ef7c9388ac00000000\"," +
                "\"isPropagated\": true}");

            TransactionData second = JsonConvert.DeserializeObject<TransactionData>(
                "{\"id\": \"b1e636e66ac0590cab93e96144267895f10916c51591f3507c260ab661cb6f0b\"," +
                "\"amount\": 23507," +
                "\"isCoinStake\": false," +
                "\"index\": 0," +
                "\"creationTime\": \"1558029576\"," +
                "\"merkleProof\": \"480000000d2b2b4b4b5359f89200c3ed8a5a21445e5b8bbc7fb8c72b0f2b3049fbd1dbb2be5a54a8ed0efe6be556dd382327c146963aa6761f67e07ef78654a6e7f9129fd2e557d8c77dc1ea05cb0a0e3b3d8b3a3035a0066bc231916681a38e47468200a5e989f8f38b658fbaec9a427eb80bdc1a15a508205b5fe2e2b403eeb8418bbff14484fb38532d9f51fe19d93fcea8d64abb893c403e3aa2cae9edb4e43e8ff8150b6fcb61b60a267c50f39115c51609f19578264461e993ab0c59c06ae636e6b115a7afaad73f5257595a85beac414e854bba087f4338d40f1275f1d1000e47be144c558a8eb7c8c775b63b2c6f326627928a96fb122d55377186bda5846991d66cf80cf18b74037058ac16063d4ffb28a49d2b56012892c9284a9bd126aed13ec55b829b0794f547d53b20fb35de2030e5e5a3a9d94c901ae274b6fc3b6d3625b806ba7cf622d11997657b0ab8d3bc2c682476c13b236c8733e890d2083f0011699c64914415783035f0794b2cc00594550c94b35884b63271e8fe96ec9443745a118cd38e48c98174bae0f51351d76b23c35b3181cf13e4a8b8f1afa68258cb04afff0f00\"," +
                "\"scriptPubKey\": \"76a91464f98576035cb19a3e68d1b2fab0b1839f59946b88ac\"," +
                "\"hex\": \"0100000001e989f8f38b658fbaec9a427eb80bdc1a15a508205b5fe2e2b403eeb8418bbff1010000006b4830450221008978b7ef4a45630765c2039507735e9b159aee53f828d01831036ed1940bfd1602203d3d2363d1ee3f3401d7feed886e20961863e3b88df15ce963c282658c41c188012103f809fdf01ed2da0e3242ec0f25fe585f31b32da4decc062a961680a8235dcc2bffffffff02d35b0000000000001976a91464f98576035cb19a3e68d1b2fab0b1839f59946b88ac3ddd0000000000001976a9147bf2a7c5e6ca4c4a4871104b21402666c2ef7c9388ac00000000\"," +
                "\"isPropagated\": true}");

            TransactionData third = JsonConvert.DeserializeObject<TransactionData>(
                "{\"id\": \"b1e636e66ac0590cab93e96144267895f10916c51591f3507c260ab661cb6f0b\"," +
                "\"amount\": 23507," +
                "\"isCoinStake\": false," +
                "\"index\": 0," +
                "\"creationTime\": \"1557856776\"," +
                "\"merkleProof\": \"480000000d2b2b4b4b5359f89200c3ed8a5a21445e5b8bbc7fb8c72b0f2b3049fbd1dbb2be5a54a8ed0efe6be556dd382327c146963aa6761f67e07ef78654a6e7f9129fd2e557d8c77dc1ea05cb0a0e3b3d8b3a3035a0066bc231916681a38e47468200a5e989f8f38b658fbaec9a427eb80bdc1a15a508205b5fe2e2b403eeb8418bbff14484fb38532d9f51fe19d93fcea8d64abb893c403e3aa2cae9edb4e43e8ff8150b6fcb61b60a267c50f39115c51609f19578264461e993ab0c59c06ae636e6b115a7afaad73f5257595a85beac414e854bba087f4338d40f1275f1d1000e47be144c558a8eb7c8c775b63b2c6f326627928a96fb122d55377186bda5846991d66cf80cf18b74037058ac16063d4ffb28a49d2b56012892c9284a9bd126aed13ec55b829b0794f547d53b20fb35de2030e5e5a3a9d94c901ae274b6fc3b6d3625b806ba7cf622d11997657b0ab8d3bc2c682476c13b236c8733e890d2083f0011699c64914415783035f0794b2cc00594550c94b35884b63271e8fe96ec9443745a118cd38e48c98174bae0f51351d76b23c35b3181cf13e4a8b8f1afa68258cb04afff0f00\"," +
                "\"scriptPubKey\": \"76a91464f98576035cb19a3e68d1b2fab0b1839f59946b88ac\"," +
                "\"hex\": \"0100000001e989f8f38b658fbaec9a427eb80bdc1a15a508205b5fe2e2b403eeb8418bbff1010000006b4830450221008978b7ef4a45630765c2039507735e9b159aee53f828d01831036ed1940bfd1602203d3d2363d1ee3f3401d7feed886e20961863e3b88df15ce963c282658c41c188012103f809fdf01ed2da0e3242ec0f25fe585f31b32da4decc062a961680a8235dcc2bffffffff02d35b0000000000001976a91464f98576035cb19a3e68d1b2fab0b1839f59946b88ac3ddd0000000000001976a9147bf2a7c5e6ca4c4a4871104b21402666c2ef7c9388ac00000000\"," +
                "\"isPropagated\": true}");

            var txList = new List<TransactionData> { first, second, third };

            if (first.IsSend == true)
            {
                Log.Info($"First tx is send to (or from): {first.ScriptPubKey.GetDestinationAddress(_walletService.WalletManager.Network)}");
            }
            else
            {
                Log.Info($"First tx is send to (or from): {first.ScriptPubKey.GetDestinationAddress(_walletService.WalletManager.Network)}");
            }

            Transactions = new ObservableCollection<Transaction> ( CreateList(txList) );
        }

        private void NavigateToMenuView()
        {
            NavigationService.Navigate<MenuViewModel>();
        }

        private void NavigateToReceiveView()
        {
            NavigationService.Navigate<ReceiveViewModel>();
        }

        private void NavigateToSendView()
        {
            NavigationService.Navigate<SendViewModel>();
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            if (_walletService.IsStarted)
            {
                _walletService.WalletManager.OnNewTransaction += WalletManager_OnWhateverTransaction;
                _walletService.WalletManager.OnNewSpendingTransaction += WalletManager_OnWhateverTransaction;
                _walletService.WalletManager.OnUpdateTransaction += WalletManager_OnWhateverTransaction;
            }
        }
        
        /// <summary>
        /// This is obviously not the final form of this... but for now,
        /// since all im doing is realoading the transactions then this is fine.
        /// </summary>
        /// <param name="sender">WalleWanager.</param>
        /// <param name="e">TranscactionData.</param>
        void WalletManager_OnWhateverTransaction(object sender, TransactionData e)
        {
            LoadTransactions();
        }
        
        public void LoadTransactions()
        {
            Transactions = new ObservableCollection<Transaction>(
                CreateList(
                    _walletService.GetCurrentAccountTransactions().OrderBy(
                        (TransactionData txData) => txData.CreationTime
                    )
                )
            );

            _walletService.Logger.Information(new string('*', 20));
            /* foreach (TransactionData transactionData in Transactions)
            {
                _walletService.Logger.Information(JsonConvert.SerializeObject(transactionData, Formatting.Indented));
            }
            _walletService.Logger.Information(new string('*', 20)); */
        }

        public void ReScan()
        {
            _walletService.ReScan(new DateTimeOffset(new DateTime(2018, 12, 1)));
        }

        public IEnumerable<Transaction> CreateList(IEnumerable<TransactionData> txList)
        {
            var result = new List<Transaction>();

            foreach (var tx in txList)
            {
                result.Add(new Transaction
                {
                    IsReceive = tx.IsReceive,
                    IsSent = tx.IsSend,
                    IsSpendable = tx.IsSpendable(),
                    IsComfirmed = tx.IsConfirmed(),
                    IsPropagated = tx.IsPropagated,
                    BlockHeight = tx.BlockHeight,
                    IsAvailable = tx.IsSpendable() ? "Available to spend" : "",
                    Memo = "In Progress",
                    Status = tx.Amount.ToString(),
                    Duration = DateTimeOffsetOperations.shortDate(tx.CreationTime)

                    // TODO: Add data for transaction model.
                });
            }
            return result;
        }
    }
}