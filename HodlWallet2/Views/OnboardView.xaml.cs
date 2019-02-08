using System;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;

using Serilog;

using HodlWallet2.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace HodlWallet2.Views
{
    [MvxContentPagePresentation(WrapInNavigationPage = true)]
    public partial class OnboardView : MvxContentPage<OnboardViewModel>
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
            Navigation.PushAsync(new PinPadView(new PinPadViewModel(ViewType.Setup)));
            _Logger.Information("Create button clicked.");
        }

        private void RecoverButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RecoverView());
            _Logger.Information("Recover button clicked.");
        }
    }
}
