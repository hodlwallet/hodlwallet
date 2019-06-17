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
        IWalletService _walletService;
        
        public MvxCommand SyncBlockChainCommand { get; private set; }
        
        public SettingsViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) : 
            base(logProvider, navigationService)
        {
            SyncBlockChainCommand = new MvxCommand(SyncBlockChain);
            _walletService = walletService;
        }

        private async void SyncBlockChain()
        {
            await NavigationService.Navigate<SyncBlockChainViewModel>();
        }
    }
}