using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using HodlWallet2.Locale;
using HodlWallet2.ViewModels;

namespace HodlWallet2
{
    public partial class OnboardPage : ContentPage
    {
        public OnboardPage()
        {
            InitializeComponent();
            SetButtonLabels();
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
