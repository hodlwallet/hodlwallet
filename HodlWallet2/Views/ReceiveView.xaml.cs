using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ZXing.Common;
using Xamarin.Essentials;

using HodlWallet2.Locale;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;

using HodlWallet2.Core.ViewModels;
using HodlWallet2.Core.Interactions;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;

namespace HodlWallet2.Views
{
    [MvxModalPresentation]
    public partial class ReceiveView : MvxContentPage<ReceiveViewModel>
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

        public ReceiveView()
        {
            InitializeComponent();
        }

        void CreateInteractionBindings()
        {
            var set = this.CreateBindingSet<ReceiveView, ReceiveViewModel>();

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



        void SetLabels()
        {
            ReceiveTitle.Text = LocaleResources.Receive_title;
            Share.Text = LocaleResources.Receive_share;
            //RequestAmount.Text = LocaleResources.Receive_requestAmount;
        }

        public async void OnCloseTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}