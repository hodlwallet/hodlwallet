using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Serilog;

using HodlWallet2.Locale;
using HodlWallet2.ViewModels;
using HodlWallet2.Utils;

namespace HodlWallet2.Views
{
    public partial class RecoverWalletEntryView : ContentPage
    {
        Wallet _Wallet;
        ILogger _Logger;

        public RecoverWalletEntryView()
        {

        }

        public RecoverWalletEntryView(RecoverWalletEntryViewModel viewModel)
        {
            InitializeComponent();
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;
            viewModel._EntryGrid = EntryGrid;
            SetLabels();
        }

        private void SetLabels()
        {
            Title.Text = LocaleResources.Recover_title;
            Header.Text = LocaleResources.Recover_entryHeader;
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            Entry completed = sender as Entry;
            string word = completed.Text.ToLower();

            if (_Wallet.IsWordInWordlist(word, "english") == false)
            {
                completed.TextColor = Color.Red;
            }
            else
            {
                completed.TextColor = Color.Black;
            }

            Entry NextEntry = this.FindByName(completed.Placeholder) as Entry;
            NextEntry?.Focus();
        }

        private void Entry_Fully_Completed(object sender, EventArgs e)
        {
            //TODO
        }
    }
}
