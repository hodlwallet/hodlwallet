using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet.Core.ViewModels;

namespace HodlWallet.UI.Views
{
    public partial class AccountSelectionView : ContentPage
    {
        public AccountSelectionView()
        {
            InitializeComponent();
        }

        async void Account_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            var vm = (AccountSelectionViewModel)BindingContext;
            vm.AccountCommand?.Execute(button.CommandParameter);

            await Navigation.PopModalAsync();
        }
    }
}
