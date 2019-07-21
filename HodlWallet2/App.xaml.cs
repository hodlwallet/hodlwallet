using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Serilog;

using HodlWallet2.Core.Services;
using System.Threading.Tasks;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HodlWallet2
{
    public partial class App : Application
    {
        readonly WalletService _WalletService;
        readonly ILogger _Logger;

        public App()
        {
            InitializeComponent();

            if (DesignMode.IsDesignModeEnabled) return;

            _WalletService = WalletService.Instance;

            _Logger = _WalletService.Logger;
        }

        protected override async void OnStart()
        {
            if (DesignMode.IsDesignModeEnabled) return;

            _Logger.Information($"App started at: {DateTime.Now}");

            if (!DesignMode.IsDesignModeEnabled)
                await Task.Run(() => _WalletService.InitializeWallet());
        }

        protected override void OnSleep()
        {
            if (DesignMode.IsDesignModeEnabled) return;

            _Logger.Information($"App sleept at: {DateTime.Now}");
        }

        protected override void OnResume()
        {
            if (DesignMode.IsDesignModeEnabled) return;

            _Logger.Information($"App resumed at: {DateTime.Now}");
        }
    }
}
