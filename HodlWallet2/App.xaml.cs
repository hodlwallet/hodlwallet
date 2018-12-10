using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HodlWallet2.Utils;
using HodlWallet2.ViewModels;
using HodlWallet2;

namespace HodlWallet2
{
    public partial class App : Application
    {
        public Wallet Wallet { get; set; }

        public App()
        {
            InitializeComponent();

            InitializeWallet();

            MainPage = new DashboardPage();
        }

        private void InitializeWallet()
        {
            Wallet = Wallet.Instance;

            // TODO Please store and run the network the user is using.
            //Wallet.Configure(walletId: "wallet_guid", network: "testnet", nodesToConnect: 4);
            Wallet.Configure(network: "testnet", nodesToConnect: 4);
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
