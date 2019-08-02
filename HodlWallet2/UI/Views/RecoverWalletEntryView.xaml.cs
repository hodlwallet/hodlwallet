using System;

using Xamarin.Forms;

using HodlWallet2.Core.Utils;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.ViewModels;
using System.Threading.Tasks;

namespace HodlWallet2.UI.Views
{
    public partial class RecoverWalletEntryView : ContentPage
    {
        IWalletService _WalletService;

        public RecoverWalletEntryView()
        {
            InitializeComponent();

            _WalletService = WalletService.Instance;

            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<RecoverWalletEntryViewModel>(this, "RecoverySeedError", ShowRecoverSeedError);
            MessagingCenter.Subscribe<RecoverWalletEntryViewModel>(this, "NavigateToRootView", async (vm) => await NavigateToRootView(vm));
        }

        void Entry_Completed(object sender, EventArgs e)
        {
            Entry completed = sender as Entry;
            if (completed.Text != null)
            {
                string word = completed.Text.ToLower();

                if (_WalletService.IsWordInWordlist(word, "english") == false)
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

        void LowercaseEntry(object sender, EventArgs e)
        {
            var entry = (Entry)sender;

            entry.Text = entry.Text.ToLower();
        }

        void ShowRecoverSeedError(RecoverWalletEntryViewModel _)
        {
            DisplayAlert(
                Constants.RECOVER_VIEW_ALERT_TITLE,
                Constants.RECOVER_VIEW_ALERT_MESSAGE,
                Constants.RECOVER_VIEW_ALERT_BUTTON
            );
        }

        async Task NavigateToRootView(RecoverWalletEntryViewModel _)
        {
            await Navigation.PushAsync(new RootView());
        }
    }
}
