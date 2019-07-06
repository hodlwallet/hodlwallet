using System;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;

using Serilog;

using HodlWallet2.Locale;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace HodlWallet2.Views
{
    [MvxContentPagePresentation(NoHistory = true)]
    public partial class DashboardView : MvxContentPage<DashboardViewModel>
    {
        public DashboardView()
        {   
            InitializeComponent();

            // FIXME. Figure out a way to use observable collections and update this automatically.
            // _Transactions.ItemsSource = _ViewModel.Transactions;
        }

        public async void OnMenuTapped(object sender, EventArgs e)
        {
            // TODO:
            /* _Transactions.ItemsSource = ViewModel.Transactions;

            _Wallet.ReScan(new DateTimeOffset(new DateTime(2018, 11, 8))); */

            //await Navigation.PushModalAsync(new MenuView());
        }

        public void SetTempLabels()
        {
            amountLabel.Text = "20 BTC";
            priceLabel.Text = "1 BTC = $4";
            sendLabel.Text = LocaleResources.Dashboard_send.ToUpper();
            receiveLabel.Text = LocaleResources.Dashboard_receive.ToUpper();
            menuLabel.Text = LocaleResources.Dashboard_menu.ToUpper();
        }
    }
}
