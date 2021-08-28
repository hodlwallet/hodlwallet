//
// AppShell.xaml.cs
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
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Models;
using HodlWallet.Core.Utils;
using HodlWallet.UI.Controls;
using HodlWallet.UI.Views;

namespace HodlWallet.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        readonly Serilog.ILogger logger;
        readonly object @lock = new();
        IWalletService WalletService => DependencyService.Get<IWalletService>();
        public ObservableCollection<AccountModel> AccountList { get; set; }  = new ();
        public ICommand SettingsCommand => new Command(async () => await Launcher.OpenAsync("//settings"));
        public ICommand GoToAccountCommand => new Command<string>((accountId) => Debug.WriteLine($"[GoToAccountCommand] Going to: //account/{accountId}"));
 
        public static bool[] isColorSelected = new bool[18];
        public static void ClearColorSelectedList()
        {
            Array.Fill(isColorSelected, false);
        }

        public static Color RandomColor()
        {
            List<int> notSelected = new();
            var rand = new Random();

            var exit = false;
            while (!exit)
            {
                for (int i = 0; i < isColorSelected.Length; i++)
                    if (!isColorSelected[i])
                        notSelected.Add(i);

                if (notSelected.Count == 0)
                    ClearColorSelectedList();
                else
                    exit = true;
            }

            return colorList[notSelected[rand.Next(notSelected.Count)]];
        }

        public static Color[] colorList = ColorPicker.colorPickerControlList;

        public AppShell()
        {
            InitializeComponent();
            logger = WalletService.Logger;
            RegisterRoutes();
            SetupDefaultTab();
            ClearColorSelectedList();
            PropertyChanged += Shell_PropertyChanged;
            AccountList.CollectionChanged += AccountsCollectionChanged;
        }

        public void ChangeTabsTo(string tabName)
        {
            var tab = tabName switch
            {
                "home" => homeTab,
                "receive" => receiveTab,
                "send" => sendTab,
                "accountSettings" => accountSettingsTab,
                _ => homeTab,
            };

            CurrentItem.CurrentItem = tab;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // TODO Add the code that inits the wallet I think?
        }

        string GetColorCodeByAccount(string accountId)
        {
            // Update the color of the account saved on storage service
            string colorStr = WalletService.GetColorByAccount(accountId);
            string colorCode = Constants.DEFAULT_ACCOUNT_COLOR_CODE;

            if (!string.IsNullOrWhiteSpace(colorStr))
            {
                int position = colorStr.IndexOf(Constants.HEX_CHAR);
                colorCode = colorStr.Substring(0, position);
            }

            return colorCode;
        }

        void AddAccountToMenu(AccountModel account)
        {
            account.AccountColorCode = GetColorCodeByAccount(account.AccountData.Id);
            string colorCode = account.AccountColorCode;
            isColorSelected[int.Parse(colorCode)] = true;
            account.CustomStyle = (Style)Resources[$"{Constants.PREFIX_NAME_STYLE_ACCOUNT_MENU}{colorCode}"];
        }

        void SetupDefaultTab()
        {
            ChangeTabsTo("home");
        }

        void AccountsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //This will get called when the collection is changed
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //  An Account was Added to the collection
                foreach (AccountModel account in e.NewItems)
                {
                    AddAccountToMenu(account);
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

        void Shell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Listen to Shell PropertyChanged event, if flyout menu is open then the property is FlyoutIsPresented.
            if (e.PropertyName.Equals("FlyoutIsPresented") && FlyoutIsPresented)
                SyncCollections();
        }

        void SyncCollections()
        {
            // Compare and sync accounts in the wallet account list that are not already into AccountList.
            AccountModel.SyncCollections(WalletService.Wallet.Accounts, AccountList);
        }

        void RegisterRoutes()
        {
            Routing.RegisterRoute("settings", typeof(SettingsView));
            Routing.RegisterRoute("create-account", typeof(CreateAccountView));
            Routing.RegisterRoute("send", typeof(SendView));
            Routing.RegisterRoute("home", typeof(HomeView));
            Routing.RegisterRoute("receive", typeof(ReceiveView));
            Routing.RegisterRoute("account-settings", typeof(AccountSettingsView));
        }
    }
}