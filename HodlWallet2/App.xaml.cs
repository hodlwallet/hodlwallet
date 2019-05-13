using System;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using NBitcoin;

using Liviano.Models;
using Liviano;
using Liviano.Managers;

using HodlWallet2.Views;
using HodlWallet2.ViewModels;
using HodlWallet2.Utils;
using MvvmCross;
using MvvmCross.Logging;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HodlWallet2
{
    public partial class App : Application
    {
        private IWalletService _wallet;
        private IMvxLog _log;

        void WalletSyncManager_OnWalletSyncedToTipOfChain(object sender, ChainedBlock e)
        {
            //_wallet.Logger.Information("Wallet finished syncing! Tip: {tip}", e.Height);
            _log.Info($"Wallet finished syncing! Tip: {e.Height}");
        }

        void WalletSyncManager_OnWalletPositionUpdate(object sender, WalletPositionUpdatedEventArgs e)
        {
            //_wallet.Logger.Information("Updated to: {tip}", e.NewPosition.Height);
            _log.Info($"Updated to: {e.NewPosition.Height}");
        }

        public App()
        {
            

            InitializeComponent();

            // If the application secure storage has the mnemonic code,
            // then the app is installed.And we just need to show the dashboard.
            if (SecureStorageProvider.HasMnemonic())
            {
#if DEBUG
                if (!Preferences.ContainsKey("FingerprintStatus") || !Preferences.ContainsKey("MnemonicStatus"))
                    SetKeys();
#endif
            }
            else
            {
                SetKeys();          
            }

            _wallet = Mvx.IoCProvider.Resolve<IWalletService>();
            _log = Mvx.IoCProvider.Resolve<IMvxLog>();
            
            
            // Add event handlers
            _wallet.OnStarted += (object sender, EventArgs args) =>
            {
                _wallet.WalletSyncManager.OnWalletPositionUpdate += WalletSyncManager_OnWalletPositionUpdate;
                _wallet.WalletSyncManager.OnWalletSyncedToTipOfChain += WalletSyncManager_OnWalletSyncedToTipOfChain;
            };

            _wallet.InitializeWallet();
        }

        private void SetKeys()
        {
            Preferences.Set("MnemonicStatus", false);
            Preferences.Set("FingerprintStatus", false);
        }

        protected override void OnStart()
        {
            //_wallet.Logger.Information("OnStart {datetime}", DateTime.Now);
            _log.Info($"OnStart {DateTime.Now}");
        }

        protected override void OnSleep()
        {
            //_wallet.Logger.Information("OnSleep {datetime}", DateTime.Now);
            _log.Info($"OnSleep {DateTime.Now}");
        }

        protected override void OnResume()
        {
            //_wallet.Logger.Information("OnResume {datetime}", DateTime.Now);
            _log.Info($"OnResume {DateTime.Now}");
        }
    }
}
