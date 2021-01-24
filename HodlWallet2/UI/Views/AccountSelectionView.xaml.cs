using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Locale;

namespace HodlWallet.UI.Views
{
    public partial class AccountSelectionView : ContentPage
    {
        public AccountSelectionView()
        {
            InitializeComponent();
            SetLabels();
        }

        void SetLabels()
        {
            Title = LocaleResources.AccountSelection_title;
            Header.Text = LocaleResources.AccountSelection_header;
            Subheader.Text = LocaleResources.AccountSelection_subheader;
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
