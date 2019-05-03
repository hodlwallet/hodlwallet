using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet2.Locale;
using HodlWallet2.ViewModels;

namespace HodlWallet2.Views
{
    public partial class LoginView : ContentPage
    {
        public LoginViewModel ViewModel { get { return BindingContext as LoginViewModel; } }

        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            SetLabel();
            viewModel._Navigation = Navigation;
            BindingContext = viewModel;
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
