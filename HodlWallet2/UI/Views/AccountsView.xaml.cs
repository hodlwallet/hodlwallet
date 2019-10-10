using System;
using System.Collections.Generic;

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

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<AccountsViewModel>(this, "NavigateToAddAccount", NavigateToAddAccount);
        }

        void NavigateToAddAccount(AccountsViewModel _)
        {

        }
    }
}