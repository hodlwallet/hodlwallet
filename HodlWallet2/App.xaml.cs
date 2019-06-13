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
        private IWalletService _Wallet;
        private IMvxLog _Log;

        void WalletSyncManager_OnWalletSyncedToTipOfChain(object sender, ChainedBlock e)
        {
            _Log.Info($"Wallet finished syncing! Tip: {e.Height}");
        }

        void WalletSyncManager_OnWalletPositionUpdate(object sender, WalletPositionUpdatedEventArgs e)
        {
            _Log.Info($"Updated to: {e.NewPosition.Height}");
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

            _Wallet = Mvx.IoCProvider.Resolve<IWalletService>();
            _Log = Mvx.IoCProvider.Resolve<IMvxLog>();
            
            // Add event handlers
            _Wallet.OnStarted += (object sender, EventArgs args) =>
            {
                _Wallet.WalletSyncManager.OnWalletPositionUpdate += WalletSyncManager_OnWalletPositionUpdate;
                _Wallet.WalletSyncManager.OnWalletSyncedToTipOfChain += WalletSyncManager_OnWalletSyncedToTipOfChain;
            };

            _Wallet.InitializeWallet();
        }

        private void SetKeys()
        {
            Preferences.Set("MnemonicStatus", false);
            Preferences.Set("FingerprintStatus", false);
        }

        protected override void OnStart()
        {
            _Log.Info($"OnStart {DateTime.Now}");
        }

        protected override void OnSleep()
        {
            _Log.Info($"OnSleep {DateTime.Now}");
        }

        protected override void OnResume()
        {
            _Log.Info($"OnResume {DateTime.Now}");
        }
    }
}
