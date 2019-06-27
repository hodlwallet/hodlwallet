using System;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using NBitcoin;

using Liviano.Models;

using HodlWallet2.Utils;
using MvvmCross;
using MvvmCross.Logging;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HodlWallet2
{
    public partial class App : Application
    {
        WalletService _Wallet;
        Serilog.ILogger _Logger;

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
