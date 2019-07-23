using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using Liviano.Exceptions;

using HodlWallet2.Core.Services;

namespace HodlWallet2.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        List<int> _Pin;

        bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

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

            _Pin = new List<int>();

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
            if (_Pin.Count - 1 >= 0)
            {
                _Pin.RemoveAt(_Pin.Count - 1);

                // Set the color to "off"
                ChangeDigitColorInteraction.Raise(new Tuple<int, bool>(_Pin.Count + 1, false));
            }
        }

        private async Task DigitTapped(int arg)
        {
            if (_Pin.Count < 6)
            {
                _Pin.Add(arg);

                // Set color to specific digit to "on"
                ChangeDigitColorInteraction.Raise(new Tuple<int, bool>(_Pin.Count, true));

                if (_Pin.Count == 6)
                {
                    await Task.Delay(305);

                    // Reset colors of all digits.
                    ResetDigitsColorInteraction.Raise();

                    string input = string.Join(string.Empty, _Pin.ToArray());
                    if (SecureStorageProvider.GetPin() == input)
                    {
                        _Pin.Clear();

                        IsLoading = true;

                        await Task.Delay(5000);

                        await NavigationService.Navigate<RootViewModel, int>((int)RootViewModel.Tabs.Home);

                        return;
                    }
                    else
                    {
                        _Pin.Clear();

                        LaunchIncorrectPinAnimationInteraction.Raise();

                        return;
                    }
                }
            }
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();

            IsLoading = false;
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
