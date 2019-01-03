using System.Linq;

using Serilog;
using Newtonsoft.Json;

using Liviano.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HodlWallet2.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private ILogger _Logger;
        private Wallet _Wallet;

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
            _Wallet = Wallet.Instance;
            _Logger = _Wallet.Logger;

            if (_Wallet.Started)
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
                    ((Wallet)sender).WalletManager.OnNewTransaction += WalletManager_OnWhateverTransaction;
                    ((Wallet)sender).WalletManager.OnNewSpendingTransaction += WalletManager_OnWhateverTransaction;
                    ((Wallet)sender).WalletManager.OnUpdateTransaction += WalletManager_OnWhateverTransaction;

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
    }
}
