using System;

using Xamarin.Forms;

using MvvmCross;
using MvvmCross.Forms.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.ViewModels;
using MvvmCross.Base;

using HodlWallet2.Locale;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Interactions;

using RecoverWalletEntryViewModel = HodlWallet2.Core.ViewModels.RecoverWalletEntryViewModel;
using Tags = HodlWallet2.Core.Utils.Tags;

namespace HodlWallet2.UI.Views
{
    public partial class RecoverWalletEntryView : MvxContentPage<RecoverWalletEntryViewModel>
    {
        IWalletService _Wallet;
        Serilog.ILogger _Logger;

        IMvxInteraction<DisplayAlertContent> _DisplayAlertInteraction;
        public IMvxInteraction<DisplayAlertContent> DisplayAlertInteraction
        {
            get => _DisplayAlertInteraction;
            set
            {
                if (_DisplayAlertInteraction != null)
                    _DisplayAlertInteraction.Requested -= OnDisplayAlertInteractionRequested;

                _DisplayAlertInteraction = value;
                _DisplayAlertInteraction.Requested += OnDisplayAlertInteractionRequested;
            }
        }

        public RecoverWalletEntryView()
        {
            InitializeComponent();

            _Wallet = Mvx.IoCProvider.Resolve<IWalletService>();
            _Logger = _Wallet.Logger;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var set = this.CreateBindingSet<RecoverWalletEntryView, RecoverWalletEntryViewModel>();
            set.Bind(this).For(view => view.DisplayAlertInteraction).To(viewModel => viewModel.DisplayAlertInteraction).OneWay();
            set.Apply();
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

        async void OnDisplayAlertInteractionRequested(object sender, MvxValueEventArgs<DisplayAlertContent> e)
        {
            var displayAlertContent = e.Value;

            await DisplayAlert(
                displayAlertContent.Title, displayAlertContent.Message, displayAlertContent.Buttons[0]
            );
        }
    }
}
