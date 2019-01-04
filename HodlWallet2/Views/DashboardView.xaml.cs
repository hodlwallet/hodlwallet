using System;

using Xamarin.Forms;

using Serilog;

using HodlWallet2.Locale;
using HodlWallet2.ViewModels;

namespace HodlWallet2.Views
{
    public partial class DashboardView : ContentPage
    {
        Wallet _Wallet;
        ILogger _Logger;

        private DashboardViewModel _ViewModel;
        public DashboardViewModel ViewModel { get => BindingContext as DashboardViewModel; }

        public DashboardView(DashboardViewModel dashboardViewModel)
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;

            _ViewModel = dashboardViewModel;
            BindingContext = _ViewModel;

            InitializeComponent();
            SetTempLabels();

            Wallet.InitializeWallet();

            // FIXME. Figure out a way to use observable collections and update this automatically.
            _Transactions.ItemsSource = _ViewModel.Transactions;
        }

        public async void OnSendTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SendView(new SendViewModel()));
        }

        public async void OnReceiveTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecieveView(new ReceiveViewModel()));
        }

        public void OnMenuTapped(object sender, EventArgs e)
        {
            _Transactions.ItemsSource = ViewModel.Transactions;
        }

        public void SetTempLabels()
        {
            amountLabel.Text = "20 BTC";
            priceLabel.Text = "1 BTC = $4";
            sendLabel.Text = LocaleResources.Dashboard_send;
            receiveLabel.Text = LocaleResources.Dashboard_receive;
            menuLabel.Text = LocaleResources.Dashboard_menu;
        }
    }
}
