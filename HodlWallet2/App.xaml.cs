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
            MainPage = new CustomNavigationPage(new DashboardView(new DashboardViewModel()));

            InitializeWallet();
        }

        private void InitializeWallet()
        {
            // FIXME Remove this with the removable code bellow.
            string guid = "736083c0-7f11-46c2-b3d7-e4e88dc38889";

            // TODO Please store and run the network the user is using.
            //Wallet.Configure(walletId: "wallet_guid", network: "testnet", nodesToConnect: 4);
            _Wallet.Configure(walletId: guid, network: "testnet");

            // FIXME Remove this code later when we have a way to create a wallet,
            // for now, the wallet is created and hardcoded
            string mnemonic = "erase fog enforce rice coil start few hold grocery lock youth service among menu life salmon fiction diamond lyrics love key stairs toe transfer";
            string password = "123456";

            if (!_Wallet.WalletExists())
            {
                _Wallet.Logger.Information("Creating wallet ({guid}) with password: {password}", guid, password);

                _Wallet.WalletManager.CreateWallet(password, guid, WalletManager.MnemonicFromString(mnemonic));

                _Wallet.Logger.Information("Wallet created.");
            }

            // NOTE Do not delete this, this is correct, the wallet should start after it being configured.
            //      Also change the date, the argument should be avoided.
            _Wallet.Start(password, new DateTimeOffset(new DateTime(2018, 12, 1)));

            _Wallet.Logger.Information("Wallet started.");

            // Add event handlers
            _Wallet.WalletSyncManager.OnWalletPositionUpdate += WalletSyncManager_OnWalletPositionUpdate;
            _Wallet.WalletSyncManager.OnWalletSyncedToTipOfChain += WalletSyncManager_OnWalletSyncedToTipOfChain;
        }

        protected override void OnStart()
        {
            _Wallet.Logger.Information("OnStart {datetime}", DateTime.Now);

            //InitializeWallet();
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
