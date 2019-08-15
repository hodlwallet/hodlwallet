using System;

using Xamarin.Forms;

using HodlWallet2.Core.Utils;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.ViewModels;
using System.Threading.Tasks;

using HodlWallet2.UI.Extensions;

namespace HodlWallet2.UI.Views
{
    public partial class RecoverWalletEntryView : ContentPage
    {
        // TODO This should be on the view model, and should be prevented to use wallet fuctions in views
        IWalletService _WalletService => DependencyService.Get<IWalletService>();

        Color _TextPrimary = (Color)Application.Current.Resources["TextPrimary"];
        Color _TextError = (Color)Application.Current.Resources["TextError"];

        public RecoverWalletEntryView()
        {
            InitializeComponent();

            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<RecoverWalletEntryViewModel>(this, "RecoverySeedError", ShowRecoverSeedError);
            MessagingCenter.Subscribe<RecoverWalletEntryViewModel>(this, "NavigateToRootView", NavigateToRootView);
        }

        void Entry_Completed(object sender, EventArgs e)
        {
            Entry completed = sender as Entry;
            if (completed.Text != null)
            {
                ValidateEntry(completed);
            }

            Entry NextEntry = this.FindByName(Tags.GetTag(completed)) as Entry;
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

            string word = entry.Text.ToLower();

            if (_WalletService.IsWordInWordlist(word, "english"))
            {
                entry.TextColor = _TextPrimary;
            }
            else
            {
                entry.TextColor = _TextError;
            }
        }

        void ShowRecoverSeedError(RecoverWalletEntryViewModel vm)
        {
            _ = this.DisplayPrompt(
                Constants.RECOVER_VIEW_ALERT_TITLE,
                Constants.RECOVER_VIEW_ALERT_MESSAGE,
                Constants.RECOVER_VIEW_ALERT_BUTTON
            );
        }

        void TryShowDoneButton()
        {
            for (int i = 1; i < 13; i++)
            {
                var entry = (Entry)FindByName($"Entry{i}");

                if (entry is null) throw new ArgumentException("Something's wrong with this...");

                if (string.IsNullOrEmpty(entry.Text)) return;
            }

            DoneButton.IsVisible = true;
        }

        void NavigateToRootView(RecoverWalletEntryViewModel _)
        {
            Navigation.PushAsync(new RootView());
        }
    }
}
