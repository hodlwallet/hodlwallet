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
using System.Threading.Tasks;
using MvvmCross.Forms.Presenters.Attributes;

namespace HodlWallet2.UI.Views
{
    [MvxModalPresentation(WrapInNavigationPage = false)]
    public partial class LoginView : MvxContentPage<LoginViewModel>
    {
        IMvxInteraction _resetDigitsColorInteraction;
        IMvxInteraction<Tuple<int, bool>> _changeDigitColorInteraction;
        IMvxInteraction _launchIncorrectPinAnimationInteraction;
        IDisposable _resetDigitsToken;
        IDisposable _changeDigitColorInteractionToken;
        IDisposable _launchIncorrectPinAnimationInteractionToken;

        static readonly Color ON_COLOR = Color.Orange;
        static readonly Color OFF_COLOR = Color.White;

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

        public IMvxInteraction LaunchIncorrectPinAnimationInteraction
        {
            get => _launchIncorrectPinAnimationInteraction;
            set
            {
                if (_launchIncorrectPinAnimationInteraction != null)
                {
                    _launchIncorrectPinAnimationInteractionToken.Dispose();
                }

                _launchIncorrectPinAnimationInteraction = value;
                _launchIncorrectPinAnimationInteractionToken = _launchIncorrectPinAnimationInteraction.WeakSubscribe(LaunchIncorrectPinAnimation);
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

        private async void LaunchIncorrectPinAnimation(object sender, EventArgs e)
        {
            // Shake ContentView Re-Enter PIN
            uint timeout = 50;
            await InputGrid.TranslateTo(-15, 0, timeout);

            await InputGrid.TranslateTo(15, 0, timeout);

            await InputGrid.TranslateTo(-10, 0, timeout);

            await InputGrid.TranslateTo(10, 0, timeout);

            await InputGrid.TranslateTo(-5, 0, timeout);

            await InputGrid.TranslateTo(5, 0, timeout);

            InputGrid.TranslationX = 0;

            await Task.Delay(500);

            ResetDigitColors(sender, e);
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
            set.Bind(this).For(v => v.LaunchIncorrectPinAnimationInteraction).To(vm => vm.LaunchIncorrectPinAnimationInteraction);
            set.Apply();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CreateInteractionsBindings();
        }
    }
}
