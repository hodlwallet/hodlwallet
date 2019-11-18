using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class AddAccountView : ContentPage
    {
        public AddAccountView(string action = null)
        {
            InitializeComponent();
        }

        async Task CreateAccountButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccountSelectionView());
        }

        async Task CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
