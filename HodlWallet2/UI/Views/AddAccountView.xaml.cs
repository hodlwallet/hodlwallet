using System;

using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class AddAccountView : ContentPage
    {
        public AddAccountView(string action = null)
        {
            InitializeComponent();
        }

        void CreateAccountButton_Clicked(object sender, EventArgs e)
        {
            // Navigation.PushAsync(new AccountSelectionView());
        }

        void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
