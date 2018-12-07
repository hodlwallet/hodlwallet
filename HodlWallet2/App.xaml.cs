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

            Wallet.Configure(network: "testnet");
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
