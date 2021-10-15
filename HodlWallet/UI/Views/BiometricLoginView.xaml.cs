using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BiometricLoginView : ContentPage
    {
        LoginViewModel ViewModel => (BiometricLoginViewModel)BindingContext;

        public BiometricLoginView(string action = null)
        {
            InitializeComponent();
            SubscribeToMessages();

            ViewModel.Action = action;

            if (ViewModel.Action == "update")
            {
                LogoFront.IsVisible = false;
                Header.Text = Locale.LocaleResources.Pin_updateHeader;
                CancelButton.IsEnabled = true;
                CancelButton.IsVisible = true;
            }
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.LoginFormVisible = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (ViewModel.IsLoading) ViewModel.IsLoading = false;

            ViewModel.LoginFormVisible = false;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<LoginViewModel>(this, "IncorrectPinAnimation", IncorrectPinAnimation);
            MessagingCenter.Subscribe<LoginViewModel>(this, "StartAppShell", StartAppShell);
        }

        async void IncorrectPinAnimation(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][IncorrectPinAnimation]");

            // Shake ContentView Re-Enter PIN
            await InputGrid.TranslateTo(-15, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(15, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(-10, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(10, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(-5, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(5, 0, incorrectPinAnimationTimeout);

            InputGrid.TranslationX = 0;

            await Task.Delay(500);
        }

        void StartAppShell(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][StartAppShell]");

            if (ViewModel.Action == "update") // Update PIN
            {
                Navigation.PushAsync(new PinPadView(new PinPadChangeView()));

                return;
            }
            else if (ViewModel.Action == "pop") // Login after logout or timeout
            {
                Navigation.PopModalAsync();

                return;
            }

            // Init app after startup, new wallet or restore
            Application.Current.MainPage = new AppShell();
        }

        void Logo_Tapped(object sender, EventArgs e)
        {
            if (SecureStorageService.HasMnemonic())
                Debug.WriteLine($"[Logo_Tapped] Seed: {SecureStorageService.GetMnemonic()}");

            Application.Current.MainPage = new ControlsDemoView();
        }

        void CancelButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}