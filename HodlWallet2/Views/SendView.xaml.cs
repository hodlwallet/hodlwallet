using NBitcoin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using Xamarin.Essentials;

using HodlWallet2.Locale;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using HodlWallet2.Core.Services;

namespace HodlWallet2.Views
{
    [MvxModalPresentation]
    public partial class SendView : MvxContentPage<SendViewModel>
    {
        public SendView()
        {
            InitializeComponent();
            SetLabels();
            ProcessClipboardContent();
        }

        async Task ProcessClipboardContent()
        {
            string content = await Clipboard.GetTextAsync();

            if (!ViewModel.IsBitcoinAddressOnClipboard(content)) return;
            
            string dialogContent = string.Format(
                LocaleResources.Send_addressDetectedOnClipboardTitle,
                content
            );
            string dialogTitle = LocaleResources.Send_addressDetectedOnClipboardTitle;
            
            bool answer = await DisplayAlert(
                dialogTitle, dialogContent, "Yes", cancel: "No"
            );

            if (!answer) return;
            
            ViewModel.AddressToSendTo = content;
        }

        void SetLabels()
        {
            SendTitle.Text = LocaleResources.Send_title;
            ToLabel.Text = LocaleResources.Send_to;
            ScanLabel.Text = LocaleResources.Send_scan;
            PasteLabel.Text = LocaleResources.Send_paste;
            AmountLabel.Text = LocaleResources.Send_amount;
            ISOLabel.Text = "USD($)"; // Localize
        }

        public async void OnCloseTapped(object sender, EventArgs e)
        {
            //TODO: Replace Modal navigation for MvvmCross.
            await Navigation.PopModalAsync();
        }

        public async void OnFaqTapped(object sender, EventArgs e)
        {
            // TODO:
        }
    }
}
