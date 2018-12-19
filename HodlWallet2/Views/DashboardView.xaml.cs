using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using HodlWallet2.Locale;
using Serilog;

using HodlWallet2.Views;
using Liviano.Models;
using Newtonsoft.Json;

namespace HodlWallet2.Views
{
    public partial class DashboardView : ContentPage
    {
        private Wallet _Wallet;
        private ILogger _Logger;
        private IOrderedEnumerable<TransactionData> _Transactions;

        public DashboardView()
        {
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;

            InitializeComponent();
            SetTempLabels();

            InitTransactions();
        }

        public void InitTransactions()
        {
            _Wallet.OnStarted += (sender, e) =>
            {
                ((Wallet)sender).WalletManager.OnNewTransaction += WalletManager_OnWhateverTransaction;
                ((Wallet)sender).WalletManager.OnNewSpendingTransaction += WalletManager_OnWhateverTransaction;
                ((Wallet)sender).WalletManager.OnUpdateTransaction += WalletManager_OnWhateverTransaction;

                LoadTransactions();
            };
        }

        /// <summary>
        /// This is obviously not the final form of this... but for now,
        /// since all im doing is realoading the transactions then this is fine.
        /// </summary>
        /// <param name="sender">WalleWanager.</param>
        /// <param name="e">TranscactionData.</param>
        void WalletManager_OnWhateverTransaction(object sender, TransactionData e)
        {
            LoadTransactions();
        }

        public void LoadTransactions()
        {
            _Transactions = _Wallet.GetCurrentAccountTransactions().OrderBy(
               (TransactionData txData) => txData.CreationTime
            );

            _Logger.Information(new string('*', 20));
            foreach (TransactionData transactionData in _Transactions)
            {
                _Logger.Information(JsonConvert.SerializeObject(transactionData, Formatting.Indented));
            }
            _Logger.Information(new string('*', 20));
        }

        public async void OnSendTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SendView(new ViewModels.SendViewModel()));
        }

        public async void OnReceiveTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecieveView(new ViewModels.ReceiveViewModel()));
        }

        public void OnMenuTapped(object sender, EventArgs e)
        {
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
