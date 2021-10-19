using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HodlWallet.Core.ViewModels;

using HodlWallet.Core.Services;
using HodlWallet.UI.Views.Demos;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using MvvmCross;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BiometricLoginView : ContentPage
    {
        BiometricLoginViewModel ViewModel => (BiometricLoginViewModel)BindingContext;

        public BiometricLoginView(string action = null)
        {
            InitializeComponent();
            SubscribeToMessages();

            ViewModel.Action = action;
            /*
            if (ViewModel.Action == "update")
            {
                LogoFront.IsVisible = false;
                Header.Text = Locale.LocaleResources.Pin_updateHeader;
                CancelButton.IsEnabled = true;
                CancelButton.IsVisible = true;
            }*/
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
            //MessagingCenter.Subscribe<BiometricLoginViewModel>(this, "IncorrectPinAnimation", IncorrectPinAnimation);
            MessagingCenter.Subscribe<BiometricLoginViewModel>(this, "StartAppShell", StartAppShell);
        }

        void IncorrectPinAnimation(BiometricLoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][IncorrectPinAnimation]");
            //await Task.Delay(500);
        }

        void StartAppShell(BiometricLoginViewModel _)
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
        
        private async void BiometricsButton_Clicked(object sender, EventArgs e)
        {
            /*
            bool availability = await CrossFingerprint.Current.IsAvailableAsync();
            if (!availability)
            {
                await DisplayAlert("Warning", "No biometrics available", "Ok");
                return;
            }
            else
            {
                await DisplayAlert("Warning", "Biometrics available", "Ok");
                return;
            }
            */

            var fpService = Mvx.Resolve<IFingerprint>(); // or use dependency injection and inject IFingerprint

            var request = new AuthenticationRequestConfiguration("Prove you have mvx fingers!", "Because without it you can't have access");
            var result = await fpService.AuthenticateAsync(request);
            if (result.Authenticated)
            {
                // do secret stuff :)
            }
            else
            {
                // not allowed to do secret stuff :(
            }




            /*
            var authResult = await CrossFingerprint.Current.AuthenticateAsync(
                new AuthenticationRequestConfiguration("Heads up!", "I would like to use biometrics"));

            if (authResult.Authenticated)
            {
                await DisplayAlert("Yaay!", "Here is the secret", "Thanks!");
                
                ViewModel.IsLoading = true;

                // DONE! We navigate to the root view
                await Task.Delay(65);

                MessagingCenter.Send(this, "StartAppShell");

                return;
            }*/
        }

        //************************************************************
        /*
        void CancelButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        */
    }
}