//
// RecoverWalletEntryView.xaml.cs
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
using System;

using Xamarin.Forms;

using HodlWallet.Core.Utils;
using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Locale;
using HodlWallet.UI.Extensions;

namespace HodlWallet.UI.Views
{
    public partial class RecoverWalletEntryView : ContentPage
    {
        RecoverWalletEntryViewModel ViewModel => BindingContext as RecoverWalletEntryViewModel;

        Color TextPrimary => (Color)Application.Current.Resources["TextPrimary"];
        Color TextError => (Color)Application.Current.Resources["TextError"];

        public RecoverWalletEntryView()
        {
            InitializeComponent();

            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<RecoverWalletEntryViewModel>(this, "RecoverySeedError", ShowRecoverSeedError);
            MessagingCenter.Subscribe<RecoverWalletEntryViewModel>(this, "InitiateAppShell", InitiateAppShell);
        }

        void Entry_Completed(object sender, EventArgs e)
        {
            var completed = sender as Entry;
            if (completed.Text != null)
            {
                ValidateEntry(completed);
            }

            var NextEntry = FindByName(Tags.GetTag(completed)) as Entry;
            NextEntry?.Focus();
        }

        void Entry_Unfocused(object sender, EventArgs e)
        {
            ValidateEntry(sender as Entry);

            TryShowDoneButton();
        }

        void LowercaseEntry(object sender, EventArgs e)
        {
            var entry = (Entry)sender;

            entry.Text = entry.Text.ToLower();

            ValidateEntry(entry);
        }

        void ValidateEntry(Entry entry)
        {
            if (entry.Text == null) return;

            var word = entry.Text;

            if (Core.Services.WalletService.IsWordInWordlist(word, ViewModel.WalletService.GetWordListLanguage()))
            {
                entry.TextColor = TextPrimary;
            }
            else
            {
                entry.TextColor = TextError;
            }
        }

        void ShowRecoverSeedError(RecoverWalletEntryViewModel vm)
        {
            _ = this.DisplayPrompt(
                LocaleResources.Recover_alertTitle,
                LocaleResources.Recover_alertHeader,
                LocaleResources.Recover_alertButton
            );
        }

        void TryShowDoneButton()
        {
            for (int i = 1; i < 13; i++)
            {
                var entry = FindByName($"Entry{i}") as Entry;

                if (string.IsNullOrEmpty(entry.Text)) return;
            }

            DoneButton.IsVisible = true;
        }

        void InitiateAppShell(RecoverWalletEntryViewModel _)
        {
            Application.Current.MainPage = new AppShell();
        }
    }
}
