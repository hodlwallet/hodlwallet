using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using HodlWallet2.Locale;
using HodlWallet2.ViewModels;

using HodlWallet2;

namespace HodlWallet2.Views
{
    public partial class OnboardPage : ContentPage
    {
        private Wallet _Wallet;

        public OnboardPage()
        {
            InitializeComponent();
            SetButtonLabels();

            _Wallet = Wallet.Instance;

            _Wallet.Logger.Information("Attached Wallet!");
        }

        private void SetButtonLabels()
        {
            createButton.Text = LocaleResources.Onboard_create.ToUpper();
            recoverButton.Text = LocaleResources.Onboard_recover.ToUpper();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SetPinPage());
        }
    }
}
