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
        bool isOpeningSend = false;
        bool isOpeningReceive = false;

        public MyBitcoinView()
        {
            InitializeComponent();
        }

        async void SendBitcoin_Tapped(object sender, EventArgs e)
        {
            if (isOpeningSend) return;
            isOpeningSend = true;

            await Navigation.PushAsync(new SendView());
            isOpeningSend = false;
        }

        async void ReceiveBitcoin_Tapped(object sender, EventArgs e)
        {
            if (isOpeningReceive) return;
            isOpeningReceive = true;

            await Navigation.PushAsync(new ReceiveView());
            isOpeningReceive = false;
        }
    }
}