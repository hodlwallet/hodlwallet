using Xamarin.Forms;

using HodlWallet2.Locale;
using HodlWallet2.Utils;

using HodlWallet2;

namespace HodlWallet2.Views
{
    public partial class PinPad : ContentPage
    {
        void On9Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new CustomNavigationPage(new DashboardPage());
        }

        public PinPad()
        {
            InitializeComponent();
        }

    }
}
