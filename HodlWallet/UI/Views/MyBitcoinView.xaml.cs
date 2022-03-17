using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyBitcoinView : ContentPage
    {
        public MyBitcoinView()
        {
            InitializeComponent();
        }

        async void SendBitcoin_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SendView());
        }

        async void ReceiveBitcoin_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReceiveView());
        }
    }
}