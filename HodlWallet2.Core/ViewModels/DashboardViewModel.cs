using System.Collections.ObjectModel;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using Liviano.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

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
    }
}