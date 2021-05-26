//
// App.xaml.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2021 HODL Wallet 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using HodlWallet.Core.Interfaces;
using HodlWallet.UI.Views;
using HodlWallet.Core.Models;

namespace HodlWallet.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
       // Serilog.ILogger logger;
        IWalletService WalletService => DependencyService.Get<IWalletService>();

        public ObservableCollection<AccountModel> AccountList { get; } = new ObservableCollection<AccountModel>();
        public ICommand SettingsCommand => new Command(async () => await Launcher.OpenAsync("//settings"));
        public ICommand CreateCommand => new Command(async () => await Launcher.OpenAsync("//create"));
        public ICommand GoToAccountCommand => new Command<string>((accountId) => Debug.WriteLine($"[GoToAccountCommand] Going to: //account/{accountId}"));

        public AppShell()
        {
            //logger = WalletService.Logger;
            InitializeComponent();
            RegisterRoutes();
            SetupAccounts();
            SetupDefaultTab();
            PropertyChanged += Shell_PropertyChanged;
        }

        public void ChangeTabsTo(string tabName)
        {
            Tab tab = tabName switch
            {
                "home" => homeTab,
                "receive" => receiveTab,
                "send" => sendTab,
                "accountSettings" => accountSettingsTab,
                _ => homeTab,
            };

            CurrentItem.CurrentItem = tab;
        }

        void SetupDefaultTab()
        {
            ChangeTabsTo("homeTab");
        }

        void RefreshAccountsList()
        {
            var itemsCount = WalletService.Wallet.Accounts.Count;
            // Build the hold list only if it has changed.
            if (Items.Count < itemsCount)
            {
                var account = WalletService.Wallet.Accounts[itemsCount - 1];
                AddMenuItems(AccountModel.FromAccountData(account));
            }
        }
        void SetupAccounts()
        {
            //  var types = new string[] { "bip44", "bip49", "bip84", "bip141", "paper" };
            LoadAccounts();
            //logger.Debug($"Setting up Accounts!!! AccountList.Count => {AccountList.Count}");
            if (AccountList.Count > 0)
            {
                foreach (var item in AccountList)
                {
                    WalletService.Logger.Information($"[AppShell] For in Accounts | Name => {item.AccountName} - Balance => {item.Balance}");
                    AddMenuItems(item);
                }
            }
        }
        void LoadAccounts()
        {
            var accounts = WalletService.Wallet.Accounts;

            foreach (var account in accounts)
            {
                AccountList.Add(AccountModel.FromAccountData(account));
            }
        }

        void AddMenuItems(AccountModel accountItem)
        {
            MenuItem mi = new()
            {
                Text = accountItem.AccountName,
                Command = GoToAccountCommand,
                CommandParameter = accountItem,
                StyleClass = new List<string> { "MenuItemLabelClass" },
            };

            Items.Add(mi);
        }

        void RegisterRoutes()
        {
            Routing.RegisterRoute("settings", typeof(SettingsView));
            Routing.RegisterRoute("create", typeof(AddAccountView));
            Routing.RegisterRoute("send", typeof(SendView));
            Routing.RegisterRoute("home", typeof(HomeView));
            Routing.RegisterRoute("receive", typeof(ReceiveView));
            Routing.RegisterRoute("account-settings", typeof(AccountSettingsView));
            Routing.RegisterRoute(nameof(CreateAccountView), typeof(CreateAccountView));
        }

        private void Shell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Listen to Shell PropertyChanged event, if flyout menu is open then the property is FlyoutIsPresented.
            if (e.PropertyName.Equals("FlyoutIsPresented"))
            {
                if (FlyoutIsPresented)
                {
                    OnFlyoutOpened();
                }
            }
        }

        void OnFlyoutOpened()
        {
            WalletService.Logger.Information("OnFlyoutOpened!!!");
            RefreshAccountsList();
        }
    }
}