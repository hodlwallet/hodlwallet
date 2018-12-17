using NBitcoin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HodlWallet2.ViewModels;
using ZXing.Mobile;

namespace HodlWallet2.Views
{
    public partial class SendView : ContentPage
    {
        public SendViewModel ViewModel { get { return BindingContext as SendViewModel; } }

        public SendView(SendViewModel sendViewModel)
        {
            InitializeComponent();
            BindingContext = sendViewModel;

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