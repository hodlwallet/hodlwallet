using System;

using Xamarin.Forms;

using NBitcoin;

using Liviano.Models;
using Liviano.Managers;

using HodlWallet2.Views;
using HodlWallet2.ViewModels;
using HodlWallet2.Utils;

namespace HodlWallet2
{
    public partial class App : Application
    {
        private Wallet _Wallet;

        void WalletSyncManager_OnWalletSyncedToTipOfChain(object sender, ChainedBlock e)
        {
            _Wallet.Logger.Information("Wallet finished syncing! Tip: {tip}", e.Height);
        }

        void WalletSyncManager_OnWalletPositionUpdate(object sender, WalletPositionUpdatedEventArgs e)
        {
            _Wallet.Logger.Information("Updated to: {tip}", e.NewPosition.Height);
        }

        public App()
        {
            _Wallet = Wallet.Instance;

            InitializeComponent();

            // Wallet.InitializeWallet();

            //If the application secure storage has the mnemonic code,
            // then the app is installed.And we just need to show the dashboard.
            //if (SecureStorageProvider.HasMnemonic())
            //{
            //    MainPage = new CustomNavigationPage(new PinPadView());
            //}
            //else
            //{
            //    MainPage = new CustomNavigationPage(new OnboardView());
            //}

            // TODO This is temporal, please use the code above.
            MainPage = new CustomNavigationPage(new OnboardView());

            // Add event handlers
            _Wallet.OnStarted += (object sender, EventArgs args) =>
            {
                _Wallet.WalletSyncManager.OnWalletPositionUpdate += WalletSyncManager_OnWalletPositionUpdate;
                _Wallet.WalletSyncManager.OnWalletSyncedToTipOfChain += WalletSyncManager_OnWalletSyncedToTipOfChain;
            };
        }

        protected override void OnStart()
        {
            _Wallet.Logger.Information("OnStart {datetime}", DateTime.Now);
        }

        protected override void OnSleep()
        {
            _Wallet.Logger.Information("OnSleep {datetime}", DateTime.Now);
        }

        protected override void OnResume()
        {
            _Wallet.Logger.Information("OnResume {datetime}", DateTime.Now);
        }
    }
}