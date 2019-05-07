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

using HodlWallet2.Locale;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace HodlWallet2.Views
{
    [MvxModalPresentation]
    public partial class SendView : MvxContentPage<SendViewModel>
    {
        public SendView()
        {
            InitializeComponent();
            //BindingContext = sendViewModel;
            SetLabels();
        }

        private void SetLabels()
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
