using System;
using HodlWallet2.Core.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class SyncBlockChainViewModel : BaseViewModel
    {
        IWalletService _WalletService;
        
        public MvxCommand StartSyncCommand { get; }
        
        public SyncBlockChainViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) 
            : base(logProvider, navigationService)
        {
            StartSyncCommand = new MvxCommand(StartSync);
        }

        private void StartSync()
        {
            //TODO: ReSync wallet
            _WalletService.ReScan(DateTimeOffset.Now);
        }
    }
}