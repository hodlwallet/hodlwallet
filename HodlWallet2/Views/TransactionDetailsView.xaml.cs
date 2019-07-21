using System;
using System.Collections.Generic;

using Xamarin.Forms;

using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.ViewModels;
using MvvmCross.Forms.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Base;

using HodlWallet2.Core.ViewModels;
using HodlWallet2.Core.Interactions;

namespace HodlWallet2.Views
{
    [MvxModalPresentation(WrapInNavigationPage = false)]
    public partial class TransactionDetailsView : MvxContentPage<TransactionDetailsViewModel>
    {
        IMvxInteraction<DisplayAlertContent> _CopiedToClipboardInteraction = new MvxInteraction<DisplayAlertContent>();
        public IMvxInteraction<DisplayAlertContent> CopiedToClipboardInteraction
        {
            get => _CopiedToClipboardInteraction;

            set
            {
                if (_CopiedToClipboardInteraction == null)
                    _CopiedToClipboardInteraction.Requested -= CopiedToClipboardInteraction_Requested;

                _CopiedToClipboardInteraction = value;
                _CopiedToClipboardInteraction.Requested += CopiedToClipboardInteraction_Requested;
            }
        }

        private async void CopiedToClipboardInteraction_Requested(object sender, MvxValueEventArgs<DisplayAlertContent> e)
        {
            var displayAlertContent = e.Value;

            await DisplayAlert(
               displayAlertContent.Title, displayAlertContent.Message, displayAlertContent.Buttons[0]
           );
        }

        public TransactionDetailsView()
        {
            InitializeComponent();
        }

        void CreateInteractionBindings()
        {
            var set = this.CreateBindingSet<TransactionDetailsView, TransactionDetailsViewModel>();

            set.Bind(this)
                .For(view => view.CopiedToClipboardInteraction)
                .To(viewModel => viewModel.CopiedToClipboardInteraction)
                .OneWay();

            set.Apply();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CreateInteractionBindings();
        }
    }
}