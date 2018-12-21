using System.Linq;

using Serilog;
using Newtonsoft.Json;

using Liviano.Models;

namespace HodlWallet2.ViewModels
{
    public class DashboardViewModel
    {
        private ILogger _Logger;
        private Wallet _Wallet;

        public IOrderedEnumerable<TransactionData> Transactions;

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
            Transactions = _Wallet.GetCurrentAccountTransactions().OrderBy(
               (TransactionData txData) => txData.CreationTime
            );

            _Logger.Information(new string('*', 20));
            foreach (TransactionData transactionData in Transactions)
            {
                _Logger.Information(JsonConvert.SerializeObject(transactionData, Formatting.Indented));
            }
            _Logger.Information(new string('*', 20));
        }
    }
}
