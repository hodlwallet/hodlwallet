using System.Collections.Generic;
using System.Threading.Tasks;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

using HodlWallet2.Core.Interfaces;
using MvvmCross.ViewModels;
using HodlWallet2.Core.Interactions;

namespace HodlWallet2.Core.ViewModels
{
    public class HomeViewModel : BaseViewModel<int>
    {
        /// <summary>
        /// Add tabs as we add views to show on the tab view
        /// </summary>
        public enum Tabs
        {
            Send,
            Receive,
            Home,
            Settings
        }

        int _InitialTab = (int)Tabs.Home;
        public int InitialTab
        {
            get => _InitialTab;
            set => SetProperty(ref _InitialTab, value);
        }

        bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        IWalletService _WalletService;

        IPrecioService _PrecioService;

        public SendTabViewModel _SendTabViewModel { get; }
        public ReceiveTabViewModel _ReceiveTabViewModel { get; }
        public HomeTabViewModel _HomeTabViewModel { get; }
        public SettingsTabViewModel _SettingsTabViewModel { get; }

        public IMvxAsyncCommand ShowInitialViewModelsCommand { get; private set; }

        MvxInteraction<SelectCurrentTab> _SelectTabInteraction = new MvxInteraction<SelectCurrentTab>();
        public IMvxInteraction<SelectCurrentTab> SelectTabInteraction => _SelectTabInteraction;

        public HomeViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService,
            IWalletService walletService,
            IPrecioService precioService)
            : base(logProvider, navigationService)
        {
            _WalletService = walletService;
            _PrecioService = precioService;
            _SendTabViewModel = new SendTabViewModel(logProvider, navigationService);
            _ReceiveTabViewModel = new ReceiveTabViewModel(logProvider, navigationService, walletService);
            _HomeTabViewModel = new HomeTabViewModel(logProvider, navigationService);

            ShowInitialViewModelsCommand = new MvxAsyncCommand(ShowInitialViewModels);
        }

        public override void Prepare(int parameter)
        {
            _InitialTab = parameter;
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            var request = new SelectCurrentTab
            {
                Tab = _InitialTab
            };

            _SelectTabInteraction.Raise(request);
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
