using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HodlWallet2.Core.Utils;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using Xamarin.Essentials;

namespace HodlWallet2.Core.ViewModels
{
    [MvxModalPresentation]
    public class SecurityCenterViewModel : BaseViewModel
    {
        private const string GrayCheck = "Assets.circle_check_gray.svg";
        private const string YellowCheck = "Assets.circle_check_yellow.svg";
        
        private string _pinCheck;
        private string _fingerprintCheck;
        private string _mnemonicCheck;

        public string PinCheck
        {
            get => _pinCheck;
            set => SetProperty(ref _pinCheck, va);
        }

        public string FingerprintCheck
        {
            get => _fingerprintCheck;
            set => SetProperty(ref _fingerprintCheck, value);
        }

        public string MnemonicCheck
        {
            get => _mnemonicCheck;
            set => SetProperty(ref _mnemonicCheck, value);
        }

        protected SecurityCenterViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
        }

        public override void Prepare()
        {
            base.Prepare();
            InitializeChecks();
        }

        private void InitializeChecks()
        {
            if (SecureStorageProvider.HasPassword())
            {
                PinCheck = YellowCheck;
            }
            else
            {
                throw new ArgumentException("Pin cannot be null after setup!");
            }
            
            if (Preferences.ContainsKey("FingerprintStatus"))
            {
                FingerprintCheck = Preferences.Get("FingerprintStatus", false)  ? YellowCheck : GrayCheck;
            }
            else
            {
                throw new ArgumentException("Fingerprint status cannot be null after setup!");
            }
            
            if (Preferences.ContainsKey("MnemonicStatus"))
            {
                MnemonicCheck = Preferences.Get("MnemonicStatus", false) ? YellowCheck : GrayCheck;
            }
            else
            {
                throw new ArgumentException("Mnemonic status cannot be null after setup!");
            }
        }
    }
}