using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using HodlWallet2.Core.Utils;
using Liviano.Exceptions;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace HodlWallet2.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private List<int> _pin;

        public MvxInteraction<Tuple<int, bool>> ChangeDigitColorInteraction { get; }
        public MvxInteraction ResetDigitsColorInteraction { get; }
        public MvxInteraction LaunchIncorrectPinAnimationInteraction { get; }

        public MvxAsyncCommand<int> DigitCommand { get; }
        public MvxCommand BackspaceCommand { get; }

        public IMvxAsyncCommand SendCommand { get; }
        public IMvxAsyncCommand ReceiveCommand { get; }

        public LoginViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            if (!SecureStorageProvider.HasPin())
            {
                throw new WalletException("Error: The wallet does not have a pin setup");
            }

            _pin = new List<int>();

            DigitCommand = new MvxAsyncCommand<int>(DigitTapped);
            BackspaceCommand = new MvxCommand(BackspaceTapped);

            SendCommand = new MvxAsyncCommand(Send);
            ReceiveCommand = new MvxAsyncCommand(Receive);

            ChangeDigitColorInteraction = new MvxInteraction<Tuple<int, bool>>();
            ResetDigitsColorInteraction = new MvxInteraction();
            LaunchIncorrectPinAnimationInteraction = new MvxInteraction();
        }

        private void BackspaceTapped()
        {
            if (_pin.Count - 1 >= 0)
            {
                _pin.RemoveAt(_pin.Count - 1);

                // Set the color to "off"
                ChangeDigitColorInteraction.Raise(new Tuple<int, bool>(_pin.Count + 1, false));
            }
        }

        private async Task DigitTapped(int arg)
        {
            if (_pin.Count < 6)
            {
                _pin.Add(arg);

                // Set color to specific digit to "on"
                ChangeDigitColorInteraction.Raise(new Tuple<int, bool>(_pin.Count, true));

                if (_pin.Count == 6)
                {
                    // Reset colors of all digits.
                    ResetDigitsColorInteraction.Raise();

                    await Task.Delay(500);

                    string input = string.Join(string.Empty, _pin.ToArray());
                    if (SecureStorageProvider.GetPin() == input)
                    {
                        _pin.Clear();

                        await NavigationService.Navigate<DashboardViewModel>();

                        return;
                    }
                    else
                    {
                        _pin.Clear();

                        LaunchIncorrectPinAnimationInteraction.Raise();

                        return;
                    }
                }
            }
        }

        private async Task Send()
        {
            await NavigationService.Navigate<SendViewModel>();
        }

        private async Task Receive()
        {
            await NavigationService.Navigate<ReceiveViewModel>();
        }
    }
}