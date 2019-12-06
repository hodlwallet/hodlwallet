using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet2.Core.ViewModels;
using HodlWallet2.UI.Locale;

namespace HodlWallet2.UI.Views
{
    public partial class AccountsView : ContentPage
    {
        public AccountsView()
        {
            InitializeComponent();
            SetLabels();
        }

        void SetLabels()
        {
            Title = LocaleResources.Accounts_title;
        }

        async void AddAccountButton_Clicked(object sender, EventArgs e)
        {
            var view = new AddAccountView();
            var nav = new NavigationPage(view);

            await Navigation.PushModalAsync(nav);
        }
    }
}