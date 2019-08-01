using System;
using HodlWallet2.Core.Services;

namespace HodlWallet2.Core.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public void ResyncWallet()
        {
            var timeToStartOn = DateTimeOffset.FromUnixTimeSeconds(
                SecureStorageService.GetSeedBirthday()
            );

            _WalletService.ReScan(timeToStartOn);
        }

        public void WipeWallet()
        {
            _WalletService.DestroyWallet();
        }
    }
}
