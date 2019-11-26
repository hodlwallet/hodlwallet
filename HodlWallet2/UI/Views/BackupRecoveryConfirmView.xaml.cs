﻿//
// BackupRecoveryConfirmView.xaml.cs
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
using Xamarin.Forms;

using HodlWallet2.Core.ViewModels;
using HodlWallet2.UI.Locale;

namespace HodlWallet2.UI.Views
{
    public partial class BackupRecoveryConfirmView : ContentPage
    {
        BackupRecoveryConfirmViewModel _ViewModel => (BackupRecoveryConfirmViewModel) BindingContext;

        public BackupRecoveryConfirmView(string[] mnemonic)
        {
            InitializeComponent();

            _ViewModel.Mnemonic = mnemonic;

            SubscribeToMessages();

            SetLabels();
        }

        void SetLabels()
        {
            Header.Text = LocaleResources.BackupConfirm_header;
            Warning.Text = LocaleResources.BackupConfirm_warning;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<BackupRecoveryConfirmViewModel>(this, "NavigateToRootView", NavigateToRootView);
        }

        void NavigateToRootView(BackupRecoveryConfirmViewModel _)
        {
            // If the recovery was launched later...
            if (Navigation.ModalStack.Count > 0)
            {
                Navigation.PopModalAsync();

                // NOTE if we want to go to home we can add this line.
                // MessagingCenter.Send(this, "ChangeCurrentPageTo", RootView.Tabs.Home);

                return;
            }

            // First time launching recovery we finish the account creation!
            Navigation.PushAsync(new RootView());
        }
    }
}
