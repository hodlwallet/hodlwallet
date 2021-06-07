﻿using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet.UI.Locale;

namespace HodlWallet.UI.Views
{
    public partial class AddAccountView : ContentPage
    {
        public AddAccountView()
        {
            InitializeComponent();
        }

        async void CreateAccountButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccountSelectionView());
        }

        async void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
