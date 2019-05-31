using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;

using HodlWallet2.Locale;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;
using MvvmCross.WeakSubscription;
using Liviano.Exceptions;

namespace HodlWallet2.Views
{
    public partial class LoginView : MvxContentPage<LoginViewModel>
    {
        private IMvxInteraction _resetDigitsColorInteraction;
        private IMvxInteraction<Tuple<int, bool>> _changeDigitColorInteraction;
        private IDisposable _resetDigitsToken;
        private IDisposable _changeDigitColorInteractionToken;

        private static readonly Color ON_COLOR = Color.Gold;
        private static readonly Color OFF_COLOR = Color.Transparent;

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
            int digit = e.Value.Item1;
            bool on = e.Value.Item2;

            BoxView pin = (BoxView) FindByName($"Pin{digit}");

            TogglePinColor(pin, on);
        }

        private void TogglePinColor(BoxView pin, bool on)
        {
            if (on)
            {
                pin.Color = ON_COLOR;
            }
            else
            {
                pin.Color = OFF_COLOR;
            }
        }

        private void ResetDigitColors(object sender, EventArgs e)
        {
            TogglePinColor(Pin1, false);
            TogglePinColor(Pin2, false);
            TogglePinColor(Pin3, false);
            TogglePinColor(Pin4, false);
            TogglePinColor(Pin5, false);
            TogglePinColor(Pin6, false);
        }

        public LoginView()
        {
            InitializeComponent();
            SetLabel();
        }

        private void SetLabel()
        {
            Header.Text = LocaleResources.Pin_enter;
        }
        
        private void CreateInteractionsBindings()
        {
            var set = this.CreateBindingSet<LoginView, LoginViewModel>();
            set.Bind(this).For(v => v.ChangeDigitColorInteraction).To(vm => vm.ChangeDigitColorInteraction);
            set.Bind(this).For(v => v.ResetDigitsColorInteraction).To(vm => vm.ResetDigitsColorInteraction);
            set.Apply();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CreateInteractionsBindings();
        }
    }
}
