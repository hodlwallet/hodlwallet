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
    public partial class AccountSettingsView : ContentPage
    {
        public AccountSettingsView()
        {
            InitializeComponent();
        }

        async void AccountInfo_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccountInfoView());
        }

        async void CoinControl_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CoinControlView());
        }
    }
}