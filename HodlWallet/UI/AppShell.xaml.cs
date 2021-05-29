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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        Serilog.ILogger logger;
        readonly object @lock = new();
        IWalletService WalletService => DependencyService.Get<IWalletService>();

        public ObservableCollection<AccountModel> AccountList = new ObservableCollection<AccountModel>();
        
        public ICommand SettingsCommand => new Command(async () => await Launcher.OpenAsync("//settings"));
        public ICommand CreateCommand => new Command(async () => await Launcher.OpenAsync("//create"));
        public ICommand GoToAccountCommand => new Command<string>((accountId) => Debug.WriteLine($"[GoToAccountCommand] Going to: //account/{accountId}"));

        public AppShell()
        {
            InitializeComponent();
            logger = WalletService.Logger;
            RegisterRoutes();
            SetupDefaultTab();
            // Important: Keep the order here of the following three instructions, if the order change
            // the accounts doesn't load correctly on scenarios where the wallet is already configured.
            PropertyChanged += Shell_PropertyChanged;
            AccountList.CollectionChanged += AccountsCollectionChanged;
            InitializeWalletServiceAccounts();
        }
        void InitializeWalletServiceAccounts()
        {
            if (WalletService.IsStarted)
            {
                logger.Debug($"InitializeWalletServiceAccounts OnConfigured - isStarted => {WalletService.IsStarted}");
                RefreshAccountsList();
            }
            else
            {
                logger.Debug($"InitializeWalletServiceAccounts OnStarted - isStarted => {WalletService.IsStarted}");
                WalletService.OnStarted += _WalletService_SetupAccounts;
            }
        }
        void _WalletService_SetupAccounts(object sender, EventArgs e)
        {
            logger.Debug($"_WalletService_SetupAccounts - walletService isStarted => {WalletService.IsStarted}");
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (@lock)
                {
                    SetupAccounts();
                }
            });
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
            int accountsCount = WalletService.Wallet.Accounts.Count;
            logger.Debug($"++++++RefreshAccount!! WalletService count => {accountsCount} - AccountList count => {AccountList.Count}");
            if (AccountList.Count < accountsCount)
            {
                SyncCollections();
            }
        }
        void SyncCollections()
        {
            // Compare and sync accounts in the wallet account list that are not already into AccountList.
            AccountModel.SyncCollections(WalletService.Wallet.Accounts, AccountList);
        }

        void AccountsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //This will get called when the collection is changed
            logger.Debug($"******AccountsCollectionChanged ADD ACTION=> {e.Action}");
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //  An Account was Added to the collection
                foreach (AccountModel account in e.NewItems)
                {
                    logger.Debug($"********AccountsCollectionChanged Item => {account.AccountName}");
                    AddMenuItems(account);
                }
            }
            // TODO: Pending implementation for another actions from NotifyCollectionChangedAction
            /*else
            {
                logger.Debug($"********AccountsCollectionChanged another ACTION=> {e.Action}");
                // An action on the Account collection
                foreach (AccountModel account in e.NewItems)
                {
                    logger.Debug($"********AccountsCollectionChanged Item => {account.AccountName}");
                }
            }*/
        }
        void SetupAccounts()
        {
            var accounts = WalletService.Wallet.Accounts;

            foreach (var account in accounts)
            {
                AccountModel acc = AccountModel.FromAccountData(account);
                logger.Debug($"SetupAccounts - Wallet service accounts => {acc.AccountName}");
                AccountList.Add(acc);
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
                    RefreshAccountsList();
                }
            }
        }
    }
}