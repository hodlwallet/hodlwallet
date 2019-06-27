using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet2.Core.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HodlWallet2
{
    public partial class App : Application
    {
        readonly WalletService _Wallet;
        readonly Serilog.ILogger _Logger;

        public App()
        {
            InitializeComponent();

            _Wallet = WalletService.Instance;
            _Logger = _Wallet.Logger;

            _Wallet.InitializeWallet();
        }

        protected override void OnStart()
        {
            _Logger.Information($"App started at: {DateTime.Now}");
        }

        protected override void OnSleep()
        {
            _Logger.Information($"App sleept at: {DateTime.Now}");
        }

        protected override void OnResume()
        {
            _Logger.Information($"App resumed at: {DateTime.Now}");
        }
    }
}
