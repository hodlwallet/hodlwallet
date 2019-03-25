﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet2.Locale;
using HodlWallet2.Utils;
using HodlWallet2.ViewModels;

namespace HodlWallet2.Views
{
    public partial class MenuView : ContentPage
    {
        public MenuView()
        {
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            MenuTitle.Text = LocaleResources.Menu_title;
            Security.Text = LocaleResources.Menu_security;
            Knowledge.Text = LocaleResources.Menu_knowledge;
            Settings.Text = LocaleResources.Menu_settings;
            LockWallet.Text = LocaleResources.Menu_lock;
        }

        public async void OnCloseTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        public async void OnSecurityClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
            await App.Current.MainPage.Navigation.PushModalAsync(new CustomNavigationPage(new SecurityCenterView(new SecurityCenterViewModel())));
        }
    }
}
