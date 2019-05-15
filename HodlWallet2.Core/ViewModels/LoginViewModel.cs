using System.Collections.Generic;
using System.Drawing;
using HodlWallet2.Core.Utils;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace HodlWallet2.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private List<int> _pin;
        public MvxCommand<int> DigitCommand { get; private set; }
        public MvxCommand BackspaceCommand { get; private set; }
        protected LoginViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            _pin = new List<int>();
            DigitCommand = new MvxCommand<int>(DigitTapped);
            BackspaceCommand = new MvxCommand(BackspaceTapped);
        }

        private void BackspaceTapped()
        {
            if (_pin.Count - 1 >= 0)
            {
                _pin.RemoveAt(_pin.Count - 1);
            }
        }

        private async void DigitTapped(int arg)
        {
            if (_pin.Count < 6)
            {
                _pin.Add(arg);
                //TODO: Set color to specific digit.
                if (_pin.Count == 6)
                {
                    //TODO: Reset colors of all digits.

                    if (SecureStorageProvider.HasPassword() == false)
                    {
                        // TODO: Throw exception
                        _pin.Clear();
                        return;
                    }
                    if (SecureStorageProvider.GetPassword() == string.Join(string.Empty, _pin.ToArray()))
                    {
                        _pin.Clear();
                        await NavigationService.Navigate<DashboardViewModel>();
                        return;
                    }
                }
            }
        }
    }
}