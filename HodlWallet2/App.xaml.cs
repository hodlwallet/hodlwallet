using System;
using Xamarin.Forms;
using HodlWallet2.ViewModels;

using Liviano.Models;
using Liviano.Managers;

namespace HodlWallet2
{
    public partial class App : Application
    {
        public Wallet Wallet { get; set; }

        public App()
        {
            InitializeComponent();

            InitializeWallet();

            MainPage = new NavigationPage(new DashboardPage());
        }

        private void InitializeWallet()
        {
            Wallet = Wallet.Instance;

            // FIXME Remove this with the removable code bellow.
            string guid = "736083c0-7f11-46c2-b3d7-e4e88dc38889";


            // TODO Please store and run the network the user is using.
            //Wallet.Configure(walletId: "wallet_guid", network: "testnet", nodesToConnect: 4);
            Wallet.Configure(walletId: guid, network: "testnet", nodesToConnect: 1);

            // FIXME Remove this code later when we have a way to create a wallet,
            // for now, the wallet is created and hardcoded
            string mnemonic = "erase fog enforce rice coil start few hold grocery lock youth service among menu life salmon fiction diamond lyrics love key stairs toe transfer";
            string password = "123456";

            if (!Wallet.WalletExists())
            {
                Wallet.Logger.Information("Creating wallet ({guid}) with password: {password}", guid, password);

                Wallet.WalletManager.CreateWallet(password, guid, WalletManager.MnemonicFromString(mnemonic));

                Wallet.Logger.Information("Wallet created.");
            }

            // NOTE Do not delete this, this is correct, the wallet should start after it being configured.
            Wallet.Start(password);

            Wallet.Logger.Information("Wallet started.");

            // Add event handlers
            Wallet.WalletSyncManager.OnWalletPositionUpdate += (object sender, WalletPositionUpdatedEventArgs e) => {
                Wallet.Logger.Information("Updated to: {tip}", e.NewPosition.Height);
            };

            Wallet.WalletSyncManager.OnWalletSyncedToTipOfChain += (object sender, NBitcoin.ChainedBlock e) => {
                Wallet.Logger.Information("Wallet finished syncing! Tip: {tip}", e.Height);
            };

        }

        protected override void OnStart()
        {
            Wallet.Logger.Information("OnStart {datetime}", DateTime.Now);
        }

        protected override void OnSleep()
        {
            Wallet.Logger.Information("OnSleep {datetime}", DateTime.Now);
        }

        protected override void OnResume()
        {
            Wallet.Logger.Information("OnResume {datetime}", DateTime.Now);
        }
    }
}
