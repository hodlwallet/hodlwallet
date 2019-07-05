using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;

using HodlWallet2.Locale;
using HodlWallet2.Core.Utils;
using MvvmCross.Commands;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace HodlWallet2.Views
{
    [MvxModalPresentation]
    public partial class MenuView : MvxContentPage<MenuViewModel>
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
    }
}
