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
    public class RootViewModel : BaseViewModel<int>
    {
        /// <summary>
        /// Add tabs as we add views to show on the tab view
        /// </summary>
        public enum Tabs
        {
            NoChange,
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

        public SendViewModel _SendViewModel { get; }
        public ReceiveViewModel _ReceiveViewModel { get; }
        public HomeViewModel _HomeViewModel { get; }
        public SettingsViewModel _SettingsViewModel { get; }

        public IMvxAsyncCommand ShowInitialViewModelsCommand { get; private set; }

        MvxInteraction<SelectCurrentTab> _SelectTabInteraction = new MvxInteraction<SelectCurrentTab>();
        public IMvxInteraction<SelectCurrentTab> SelectTabInteraction => _SelectTabInteraction;

        public RootViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService,
            IWalletService walletService,
            IPrecioService precioService)
            : base(logProvider, navigationService)
        {
            _WalletService = walletService;
            _PrecioService = precioService;

            _SendViewModel = new SendViewModel(logProvider, navigationService, walletService, precioService);
            _ReceiveViewModel = new ReceiveViewModel(logProvider, navigationService, walletService);
            _HomeViewModel = new HomeViewModel(logProvider, navigationService, walletService, precioService);
            _SettingsViewModel = new SettingsViewModel(logProvider, navigationService);

            ShowInitialViewModelsCommand = new MvxAsyncCommand(ShowInitialViewModels);
        }

        //public RootViewModel(
        //    IMvxLogProvider logProvider,
        //    IMvxNavigationService navigationService)
        //    : base(logProvider, navigationService)
        //{
        //    ShowInitialViewModelsCommand = new MvxAsyncCommand(ShowInitialViewModels);
        //}

        public override void Prepare(int parameter)
        {
            _InitialTab = parameter;
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            if (_InitialTab == (int)Tabs.NoChange)
                return;

            var request = new SelectCurrentTab
            {
                Tab = _InitialTab
            };

            _SelectTabInteraction.Raise(request);
        }

        async Task ShowInitialViewModels()
        {
            var tasks = new List<Task>();

            tasks.Add(NavigationService.Navigate<SendViewModel>());
            tasks.Add(NavigationService.Navigate<ReceiveViewModel>());
            tasks.Add(NavigationService.Navigate<HomeViewModel>());
            tasks.Add(NavigationService.Navigate<SettingsViewModel>());

            await Task.WhenAll(tasks);
        }

    }
}
