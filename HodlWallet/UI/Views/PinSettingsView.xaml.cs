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
    public partial class PinSettingsView : ContentPage
    {
        public PinSettingsView()
        {
            InitializeComponent();
        }
        private void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void UpdatePin_Clicked(object sender, EventArgs e)
        {

        }

        private void PinSpendingLimits_Clicked(object sender, EventArgs e)
        {

        }
    }
}