using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Serilog;

using HodlWallet2.Locale;
using HodlWallet2.Utils;
using HodlWallet2.ViewModels;
using MvvmCross.Forms.Views;
using PinPadViewModel = HodlWallet2.Core.ViewModels.PinPadViewModel;
using RecoverWalletEntryViewModel = HodlWallet2.Core.ViewModels.RecoverWalletEntryViewModel;
using Tags = HodlWallet2.Shared.Controls.Utils.Tags;

namespace HodlWallet2.Views
{
    public partial class RecoverWalletEntryView : MvxContentPage<RecoverWalletEntryViewModel>
    {
        Wallet _Wallet;
        ILogger _Logger;
        string mnemonic;

        public RecoverWalletEntryView()
        {
            InitializeComponent();
            //_Wallet = Wallet.Instance;
            //_Logger = _Wallet.Logger;
            //viewModel._EntryGrid = EntryGrid;
            //SetLabels();
        }

        private void SetLabels()
        {
//            Title.Text = LocaleResources.Recover_title;
//            Header.Text = LocaleResources.Recover_entryHeader;
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            Entry completed = sender as Entry;
            if (completed.Text != null)
            {
                string word = completed.Text.ToLower();

                if (_Wallet.IsWordInWordlist(word, "english") == false)
                {
                    completed.TextColor = Color.Red;
                }
                else
                {
                    completed.TextColor = Color.Black;
                }
            }

            Entry NextEntry = this.FindByName(Tags.GetTag(completed)) as Entry;
            NextEntry?.Focus();
        }

        private void Entry_Fully_Completed(object sender, EventArgs e)
        {

            for (int i = 1; i <= 12; i++)
            {
                string x_Name = "Entry" + i.ToString();
                Entry currentEntry = this.FindByName(x_Name) as Entry;

                if (_Wallet.IsWordInWordlist(currentEntry.Text) == false)
                {
                    _Logger.Information("User input not found in wordlist.");
                    DisplayAlert(LocaleResources.Recover_alertTitle, LocaleResources.Recover_alertHeader, LocaleResources.Recover_alertButton);
                    return;
                }

                mnemonic += i < 12 ? currentEntry.Text + " " : currentEntry.Text;
            }

            if (_Wallet.IsVerifyChecksum(mnemonic, "english") == false)
            {
                DisplayAlert(LocaleResources.Recover_alertTitle, LocaleResources.Recover_alertHeader, LocaleResources.Recover_alertButton);
                return;
            }

            // TODO: Create wallet
            SecureStorageProvider.SetMnemonic(mnemonic);
            //Navigation.PushAsync(new PinPadView(new PinPadViewModel(ViewType.Recover)));
        }
    }
}
