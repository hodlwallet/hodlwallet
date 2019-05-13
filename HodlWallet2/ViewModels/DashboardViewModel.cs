using System.Linq;

using Serilog;
using Newtonsoft.Json;

using Liviano.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using HodlWallet2.Core.Services;

namespace HodlWallet2.ViewModels
{
    [Obsolete]
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private Serilog.ILogger _Logger;
        private WalletService _Wallet;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<TransactionData> _Transactions = new ObservableCollection<TransactionData>();
        public ObservableCollection<TransactionData> Transactions
        {
            get
            {
                return _Transactions;
            }

            set
            {
                _Transactions = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Transactions)));
            }
        }

        public DashboardViewModel()
        {
            _Wallet = WalletService.Instance;
            _Logger = _Wallet.Logger;

            if (_Wallet.IsStarted)
            {
                _Wallet.WalletManager.OnNewTransaction += WalletManager_OnWhateverTransaction;
                _Wallet.WalletManager.OnNewSpendingTransaction += WalletManager_OnWhateverTransaction;
                _Wallet.WalletManager.OnUpdateTransaction += WalletManager_OnWhateverTransaction;

                LoadTransactions();
            }
            else
            {
                _Wallet.OnStarted += (sender, e) =>
                {
                    ((WalletService)sender).WalletManager.OnNewTransaction += WalletManager_OnWhateverTransaction;
                    ((WalletService)sender).WalletManager.OnNewSpendingTransaction += WalletManager_OnWhateverTransaction;
                    ((WalletService)sender).WalletManager.OnUpdateTransaction += WalletManager_OnWhateverTransaction;

                    LoadTransactions();
                };
            }

            if (_Wallet.GetCurrentAccountTransactions().Count<TransactionData>() > 0)
                LoadTransactions();
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
            _Transactions = new ObservableCollection<TransactionData>(
                _Wallet.GetCurrentAccountTransactions().OrderBy(
                    (TransactionData txData) => txData.CreationTime
                )
            );

            _Logger.Information(new string('*', 20));
            foreach (TransactionData transactionData in _Transactions)
            {
                _Logger.Information(JsonConvert.SerializeObject(transactionData, Formatting.Indented));
            }
            _Logger.Information(new string('*', 20));
        }

        public void ReScan()
        {
            _Wallet.ReScan(new DateTimeOffset(new DateTime(2018, 12, 1)));
        }
    }
}
