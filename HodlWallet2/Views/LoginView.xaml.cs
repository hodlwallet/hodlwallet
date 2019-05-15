using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;

using HodlWallet2.Locale;
using MvvmCross.Forms.Views;

namespace HodlWallet2.Views
{
    public partial class LoginView : MvxContentPage<LoginViewModel>
    {
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
