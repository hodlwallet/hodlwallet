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

        public LoginView()
        {
            InitializeComponent();
            SetLabel();
        }

        private void SetLabel()
        {
            Header.Text = LocaleResources.Pin_enter;
        }
    }
}
