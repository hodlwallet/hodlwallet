﻿//
// RootView.xaml.cs
//
// Copyright (c) 2019 HODL Wallet
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
using System.ComponentModel;

using Xamarin.Forms;

using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Controls;
using HodlWallet.UI.Effects;

namespace HodlWallet.UI.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class RootView : TabbedPage
    {
        public enum Tabs { Accounts, Send, Home, Receive, Settings };

        public RootView()
        {
            InitializeComponent();
            SubscribeToMessages();

            ChangeTabTo(Tabs.Home);
        }

        void SubscribeToMessages()
        {
            // Views (this is unused, but could be an example)
            MessagingCenter.Subscribe<SendView, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<PromptView>(this, "HideTabbar", HideTabbar);
            MessagingCenter.Subscribe<PromptView>(this, "ShowTabbar", ShowTabbar);

            // View Models
            MessagingCenter.Subscribe<SendViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<ReceiveViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<SettingsViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<HomeViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<AccountsViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            // Add more view models, as needed though

            // Add yours here.
        }

        void ChangeCurrentPageTo(BaseViewModel _, Tabs tab)
        {
            ChangeTabTo(tab);
        }

        void ChangeCurrentPageTo(ContentPage _, Tabs tab)
        {
            ChangeTabTo(tab);
        }

        void ChangeTabTo(Tabs tab)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                CurrentPage = Children[(int)tab];
            });
        }

        void HideTabbar(PromptView _)
        {
            Effects.Add(Effect.Resolve($"HodlWallet.{nameof(HideTabbarEffect)}"));
        }

        void ShowTabbar(PromptView _)
        {
            // TODO this obviously has issues when we add more effects to it...
            // if we ever do!
            Effects.Clear();
        }
    }
}