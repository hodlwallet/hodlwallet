using Xamarin.Forms;

using HodlWallet2.Utils;


namespace HodlWallet2.Views
{
    public partial class PinPadView : ContentPage
    {
        void On9Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new CustomNavigationPage(new DashboardView());
        }

        public PinPadView()
        {
            InitializeComponent();
        }

    }
}
