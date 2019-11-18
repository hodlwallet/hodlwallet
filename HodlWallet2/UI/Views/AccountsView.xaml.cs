using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet2.Core.ViewModels;

namespace HodlWallet2.UI.Views
{
    public partial class AccountsView : ContentPage
    {
        public AccountsView()
        {
            InitializeComponent();
        }

        async void AddAccountButton_Clicked(object sender, EventArgs e)
        {
            var view = new AddAccountView();
            var nav = new NavigationPage(view);

            await Navigation.PushModalAsync(nav);
        }
    }
}