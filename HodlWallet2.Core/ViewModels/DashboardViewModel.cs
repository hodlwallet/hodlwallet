using System;
using System.Collections.ObjectModel;
using System.Linq;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
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

        private ObservableCollection<TransactionData> _transactions;

        public ObservableCollection<TransactionData> Transactions
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
            Transactions = new ObservableCollection<TransactionData>(
                _walletService.GetCurrentAccountTransactions().OrderBy(
                    (TransactionData txData) => txData.CreationTime
                )
            );

            _walletService.Logger.Information(new string('*', 20));
            foreach (TransactionData transactionData in Transactions)
            {
                _walletService.Logger.Information(JsonConvert.SerializeObject(transactionData, Formatting.Indented));
            }
            _walletService.Logger.Information(new string('*', 20));
        }

        public void ReScan()
        {
            _walletService.ReScan(new DateTimeOffset(new DateTime(2018, 12, 1)));
        }
    }
}