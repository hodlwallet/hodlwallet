using System.Collections.Generic;
using System.Threading.Tasks;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

using HodlWallet2.Core.Interfaces;

namespace HodlWallet2.Core.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        public SendTabViewModel _SendTabViewModel { get; }
        public ReceiveTabViewModel _ReceiveTabViewModel { get; }
        public HomeTabViewModel _HomeTabViewModel { get; }
        public SettingsTabViewModel _SettingsTabViewModel { get; }

        public IMvxAsyncCommand ShowInitialViewModelsCommand { get; private set; }

        public HomeViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService,
            IWalletService walletService,
            IPrecioService precioService)
            : base(logProvider, navigationService)
        {
            _SendTabViewModel = new SendTabViewModel(logProvider, navigationService);
            _ReceiveTabViewModel = new ReceiveTabViewModel(logProvider, navigationService);
            _HomeTabViewModel = new HomeTabViewModel(logProvider, navigationService);

            ShowInitialViewModelsCommand = new MvxAsyncCommand(ShowInitialViewModels);
        }

        async Task ShowInitialViewModels()
        {
            var tasks = new List<Task>();

            tasks.Add(NavigationService.Navigate<SendTabViewModel>());
            tasks.Add(NavigationService.Navigate<ReceiveTabViewModel>());
            tasks.Add(NavigationService.Navigate<HomeTabViewModel>());
            tasks.Add(NavigationService.Navigate<SettingsTabViewModel>());

            await Task.WhenAll(tasks);
        }

    }
}