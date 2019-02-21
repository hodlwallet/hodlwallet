using NBitcoin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;

using HodlWallet2.ViewModels;
using HodlWallet2.Locale;

namespace HodlWallet2.Views
{
    public partial class SendView : ContentPage
    {
        public SendViewModel ViewModel { get { return BindingContext as SendViewModel; } }

        public SendView(SendViewModel sendViewModel)
        {
            InitializeComponent();
            BindingContext = sendViewModel;
            SetLabels();
        }

        private void SetLabels()
        {
            Title = LocaleResources.Send_title;
            ToLabel.Text = LocaleResources.Send_to;
            ScanLabel.Text = LocaleResources.Send_scan;
            PasteLabel.Text = LocaleResources.Send_paste;
            AmountLabel.Text = LocaleResources.Send_amount;
            ISOLabel.Text = "USD($)"; // Localize
        }

        public async void Scan(object sender, System.EventArgs e)
        {
            var address = await ViewModel.Scan();
            sendAddress.Text = address;
        }

        public async void Send(object sender, System.EventArgs e)
        {
            await ViewModel.Send();
        }
    }
}
