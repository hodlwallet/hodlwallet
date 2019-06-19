using System;
using HodlWallet2.Core.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        IWalletService _WalletService;
        
        public MvxCommand SyncBlockChainCommand { get; }
        
        public SettingsViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) : 
            base(logProvider, navigationService)
        {
            SyncBlockChainCommand = new MvxCommand(SyncBlockChain);
            _WalletService = walletService;
        }

        private async void SyncBlockChain()
        {
            await NavigationService.Navigate<SyncBlockChainViewModel>();
        }
    }
}