//
// RecoverView.xaml.cs
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
using HodlWallet.Core.Services;

namespace HodlWallet.UI.Views
{
    public partial class RecoverView : ContentPage
    {
        RecoverViewModel ViewModel => BindingContext as RecoverViewModel;

        Color Fg => (Color)Application.Current.Resources["Fg"];
        Color FgError => (Color)Application.Current.Resources["FgError"];

        public RecoverView()
        {
            InitializeComponent();

            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<RecoverViewModel>(this, "RecoverySeedError", ShowRecoverSeedError);
            MessagingCenter.Subscribe<RecoverViewModel>(this, "ShowRecoverAccountType", ShowRecoverAccountType);
        }

        void Entry_Completed(object sender, EventArgs e)
        {
            var completed = sender as Entry;
            if (completed.Text != null)
                ValidateEntry(completed);

            var NextEntry = FindByName(Tags.GetTag(completed)) as Entry;
            NextEntry?.Focus();
        }

        void Entry_Unfocused(object sender, EventArgs e)
        {
            ValidateEntry(sender as Entry);

            ToggleNextButton();
        }

        void LowercaseEntry(object sender, EventArgs e)
        {
            var entry = (Entry)sender;

            entry.Text = entry.Text.ToLower();

            ValidateEntry(entry);

            ToggleNextButton();
        }

        void ValidateEntry(Entry entry)
        {
            if (entry.Text == null) return;

            var word = entry.Text;

            if (WalletService.IsWordInWordlist(word, ViewModel.WalletService.GetWordListLanguage()))
                entry.TextColor = Fg;
            else
                entry.TextColor = FgError;
        }

        void ShowRecoverSeedError(RecoverViewModel vm)
        {
            _ = this.DisplayPrompt(
                LocaleResources.Recover_alertTitle,
                LocaleResources.Recover_alertHeader,
                LocaleResources.Recover_alertButton
            );
        }

        void ToggleNextButton()
        {
            for (int i = 1; i < 13; i++)
            {
                var entry = FindByName($"Entry{i}") as Entry;

                if (string.IsNullOrEmpty(entry.Text))
                {
                    NextButton.IsVisible = false;

                    return;
                }
            }

            NextButton.IsVisible = true;
        }

        void ShowRecoverAccountType(RecoverViewModel _)
        {
            Navigation.PushAsync(new RecoverAccountTypeView());
        }

        void DebugMnemonic_Tapped(object sender, EventArgs e)
        {
#if DEBUG
            //var mnemonic = "abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon about".Split(' ');
            var mnemonic = "level mango alcohol plunge cup hello wagon reason silver lion kit shuffle".Split(' ');

            for (int i = 0; i < 12; i++) (FindByName($"Entry{i + 1}") as Entry).Text = mnemonic[i];
#endif
        }
    }
}
