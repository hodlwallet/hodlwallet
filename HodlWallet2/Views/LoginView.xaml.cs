using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;

using HodlWallet2.Locale;
using MvvmCross.Base;
using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;
using MvvmCross.WeakSubscription;

namespace HodlWallet2.Views
{
    public partial class LoginView : MvxContentPage<LoginViewModel>
    {
        private IMvxInteraction _resetDigitsColorInteraction;
        private IMvxInteraction<Tuple<int, bool>> _changeDigitColorInteraction;
        private IDisposable _resetDigitsToken;
        private IDisposable _changeDigitColorInteractionToken;

        public IMvxInteraction ResetDigitsColorInteraction
        {
            get => _resetDigitsColorInteraction;
            set
            {
                if (_resetDigitsColorInteraction != null)
                {
                    _resetDigitsToken.Dispose();
                }

                _resetDigitsColorInteraction = value;
                _resetDigitsToken = _resetDigitsColorInteraction.WeakSubscribe(ResetDigitColors);
            }
        }

        public IMvxInteraction<Tuple<int, bool>> ChangeDigitColorInteraction
        {
            get => _changeDigitColorInteraction;
            set
            {
                if (_changeDigitColorInteraction != null)
                {
                    _changeDigitColorInteractionToken.Dispose();
                }

                _changeDigitColorInteraction = value;
                _changeDigitColorInteractionToken = _changeDigitColorInteraction.WeakSubscribe(ChangeDigitColor);
            }
        }

        private void ChangeDigitColor(object sender, MvxValueEventArgs<Tuple<int, bool>> e)
        {
            //TODO: Change digit color based on argument.
        }

        private void ResetDigitColors(object sender, EventArgs e)
        {
            //TODO: Reset UI colors here.
        }

        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            SetLabel();
        }

        private void SetLabel()
        {
            Header.Text = LocaleResources.Pin_enter;
        }

        public async void OnSendTapped(object sender, EventArgs e)
        {
            //await Navigation.PushModalAsync(new SendView(new SendViewModel()));
        }

        public async void OnReceiveTapped(object sender, EventArgs e)
        {
            //await Navigation.PushModalAsync(new ReceiveView(new ReceiveViewModel()));
        }
    }
}
