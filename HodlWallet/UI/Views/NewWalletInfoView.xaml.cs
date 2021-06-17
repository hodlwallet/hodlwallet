using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewWalletInfoView : ContentPage
    {
        public NewWalletInfoView()
        {
            InitializeComponent();
        }

        void WalletInfoButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AcknowledgeRiskView());
        }
    }
}