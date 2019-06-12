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
using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IWalletService _walletService;

        public string SendText => "Send";
        public string ReceiveText => "Receive";
        public string SyncText => "SYNCING";
        public string DateText => DateTimeOffset.UtcNow.UtcDateTime.ToShortDateString() + ", Block: 478045";
        public double Progress => 0.7;
        public bool IsVisible => true;

        const string SENT_COLOR = "#A3A8AD";
        const string UNSENT_COLOR = "#DAAB28";

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
                _walletService.WalletSyncManager.OnWalletPositionUpdate += WalletSyncManager_OnSyncProgressUpdate;
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

        void WalletSyncManager_OnSyncProgressUpdate(object sender, WalletPositionUpdatedEventArgs e)
        {
            // TODO: Update Progress During Sync
            // e.g. Progress = e.NewPosition.Height / _walletService.CurrentBlockHeight
            //      Date = e.NewPosition.GetMedianTimePast().UtcDateTime.ToShortDateString() + ", Block: " + e.NewPosition.Height.ToString();
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
                    Status = GetStatus(tx),
                    StatusColor = tx.IsSend == true
                                    ? Color.FromHex(SENT_COLOR)
                                    : Color.FromHex(UNSENT_COLOR),
                    // TODO: Implement Send and Receive
                    // AtAddress = WalletService.GetAddressFromTranscation(tx),
                    Duration = DateTimeOffsetOperations.shortDate(tx.CreationTime)
                });

                _walletService.Logger.Information(JsonConvert.SerializeObject(tx, Formatting.Indented));
            }
            return result;
        }

        private string GetStatus(TransactionData tx)
        {
            switch (tx.IsSend)
            {
                case true:
                    return "Sent BTC " + tx.Amount.ToString();
                case false:
                case null:
                    switch (tx.IsReceive)
                    {
                        case true:
                            return "Received BTC " + tx.Amount.ToString();
                        case false:
                        case null:
                            return "Send and Receive is NULL";
                    }
                    break;
            }
            return "";
        }
    }
}