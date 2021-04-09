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
using System.Diagnostics;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using HodlWallet.UI.Views;

namespace HodlWallet.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        // XXX
        public IEnumerable<string> Accounts = new List<string>() { "Account 1", "Account 2", "Account 3" };
        public ICommand SettingsCommand => new Command(async () => await Launcher.OpenAsync("//settings"));
        public ICommand CreateCommand => new Command(async () => await Launcher.OpenAsync("//create"));
        public ICommand GoToAccountCommand => new Command<string>((accountId) => Debug.WriteLine($"[GoToAccountCommand] Going to: //account/{accountId}"));


        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
            SetupAccounts();
            SetupDefaultTab();
        }

        void SetupDefaultTab()
        {
            ChangeTabsTo("homeTab");
        }

        void ChangeTabsTo(string tabName)
        {
            Tab tab = tabName switch
            {
                "homeTab" => homeTab,
                "receiveTab" => receiveTab,
                "sendTab" => sendTab,
                "accountSettingsTab" => accountSettingsTab,
                _ => homeTab,
            };

            CurrentItem.CurrentItem = tab;
        }

        void SetupAccounts()
        {
            foreach (var item in Accounts)
            {
                MenuItem mi = new()
                {
                    Text = item,
                    Command = GoToAccountCommand,
                    CommandParameter = item,
                    StyleClass = new List<string> { "MenuItemLabelClass" },
                };

                Items.Add(mi);
            }
        }

        void RegisterRoutes()
        {
            Routing.RegisterRoute("settings", typeof(SettingsView));
            Routing.RegisterRoute("create", typeof(AddAccountView));
            Routing.RegisterRoute("send", typeof(SendView));
            Routing.RegisterRoute("home", typeof(HomeView));
            Routing.RegisterRoute("receive", typeof(ReceiveView));
            Routing.RegisterRoute("account-settings", typeof(AccountSettingsView));
        }
    }
}