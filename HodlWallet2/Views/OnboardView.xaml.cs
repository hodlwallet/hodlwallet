using System;
using Xamarin.Forms;

using Serilog;

namespace HodlWallet2.Views
{
    public partial class OnboardView : ContentPage
    {
        Wallet _Wallet;
        ILogger _Logger;

        public OnboardView()
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;

            InitializeComponent();
        }

        private void CreateButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SetPinView());
        }

        private void RecoverButton_Clicked(object sender, EventArgs e)
        {
            _Logger.Information("Recover button clicked.");
        }
    }
}
