using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HodlWallet2.Core.Utils;
using MvvmCross.Commands;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using Xamarin.Essentials;

namespace HodlWallet2.Core.ViewModels
{
    [MvxModalPresentation]
    public class SecurityCenterViewModel : BaseViewModel
    {
        const string GrayCheck = "Assets.circle_check_gray.svg";
        const string YellowCheck = "Assets.circle_check_yellow.svg";
        
        string _PinCheck;
        string _FingerprintCheck;
        string _MnemonicCheck;
        
        public MvxCommand CloseCommand { get; }
        public MvxCommand FaqCommand { get; }
        public MvxCommand PinCommand { get; }
        public MvxCommand FingerprintCommand { get; }
        public MvxCommand MnemonicCommand { get; }
        
        //TODO: Add string properties here
        

        public string PinCheck
        {
            get => _PinCheck;
            set => SetProperty(ref _PinCheck, value);
        }

        public string FingerprintCheck
        {
            get => _FingerprintCheck;
            set => SetProperty(ref _FingerprintCheck, value);
        }

        public string MnemonicCheck
        {
            get => _MnemonicCheck;
            set => SetProperty(ref _MnemonicCheck, value);
        }

        protected SecurityCenterViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService) 
            : base(logProvider, navigationService)
        {
            CloseCommand = new MvxCommand(Close);
            FaqCommand = new MvxCommand(FaqTapped);
            PinCommand = new MvxCommand(PinTapped);
            FingerprintCommand = new MvxCommand(FingerprintTapped);
            MnemonicCommand = new MvxCommand(MnemonicTapped);
        }

        private async void MnemonicTapped()
        {
            await NavigationService.Navigate<BackupViewModel>();
        }

        private void FingerprintTapped()
        {
            //TODO: Implement biometrics
        }

        private async void PinTapped()
        {
            await NavigationService.Navigate<PinPadViewModel>(); //TODO: Pass UPDATE parameter
        }

        private void FaqTapped()
        {
            throw new NotImplementedException();
        }

        private void Close()
        {
            throw new NotImplementedException();
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            InitializeChecks();
        }

        private void InitializeChecks()
        {
            if (SecureStorageProvider.HasPin())
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