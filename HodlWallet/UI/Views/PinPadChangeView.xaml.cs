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
    public partial class PinChangedView : ContentPage
    {
        public PinChangedView()
        {
            InitializeComponent();
        }

        private async void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//account-settings");
        }
    }
}