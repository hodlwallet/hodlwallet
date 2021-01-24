using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Locale;

namespace HodlWallet.UI.Views
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