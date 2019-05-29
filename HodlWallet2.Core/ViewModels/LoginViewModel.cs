using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using HodlWallet2.Core.Utils;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace HodlWallet2.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private List<int> _pin;

        private readonly MvxInteraction<Tuple<int, bool>> _changeDigitColorInteraction;
        private readonly MvxInteraction _resetDigitsColorInteraction;

        public IMvxInteraction<Tuple<int, bool>> ChangeDigitColorInteraction => _changeDigitColorInteraction;
        public MvxCommand<int> DigitCommand { get; private set; }
        public MvxCommand BackspaceCommand { get; private set; }

        public IMvxAsyncCommand SendCommand { get; private set; }
        public IMvxAsyncCommand ReceiveCommand { get; private set; }

        public LoginViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            _pin = new List<int>();
            DigitCommand = new MvxCommand<int>(DigitTapped);
            BackspaceCommand = new MvxCommand(BackspaceTapped);
            _changeDigitColorInteraction = new MvxInteraction<Tuple<int, bool>>();
            _resetDigitsColorInteraction = new MvxInteraction();
            SendCommand = new MvxAsyncCommand(Send);
            ReceiveCommand = new MvxAsyncCommand(Receive);
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
                // Set color to specific digit.
                _changeDigitColorInteraction.Raise(new Tuple<int, bool>(_pin.Count, true));
                if (_pin.Count == 6)
                {
                    // Reset colors of all digits.
                    _resetDigitsColorInteraction.Raise();
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