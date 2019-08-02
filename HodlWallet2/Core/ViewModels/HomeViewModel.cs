using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Essentials;

using Newtonsoft.Json;

using Liviano;
using Liviano.Models;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.Utils;
using HodlWallet2.Core.Services;
using NBitcoin;
using System.Windows.Input;
using NBitcoin.Protocol;

namespace HodlWallet2.Core.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        Serilog.ILogger _Logger;

        public string SendText => "Send";
        public string ReceiveText => "Receive";
        public string MenuText => "Menu";
        public string SyncTitleText => "SYNCING";

        decimal _AmountBTC;
        decimal _Amount;
        float _NewRate;
        float _OldRate;
        bool _IsBtcEnabled;
        object _CurrentTransaction;

        public object CurrentTransaction
        {
            get => _CurrentTransaction;
            set => SetProperty(ref _CurrentTransaction, value);
        }

        public bool IsBtcEnabled
        {
            get => _IsBtcEnabled;
            set => SetProperty(ref _IsBtcEnabled, value);
        }

        public decimal Amount
        {
            get => _Amount;
            set => SetProperty(ref _Amount, value);
        }

        private object _Lock = new object();
        public ObservableCollection<TransactionModel> Transactions { get; } = new ObservableCollection<TransactionModel>();

        string _PriceText;
        public string PriceText
        {
            get => _PriceText;
            set => SetProperty(ref _PriceText, value);
        }

        string _SyncDateText;
        public string SyncDateText
        {
            get => _SyncDateText;
            set => SetProperty(ref _SyncDateText, value);
        }

        double _SyncCurrentProgress;
        public double SyncCurrentProgress
        {
            get => _SyncCurrentProgress;
            set => SetProperty(ref _SyncCurrentProgress, value);
        }

        bool _SyncIsVisible;
        public bool SyncIsVisible
        {
            get => _SyncIsVisible;
            set => SetProperty(ref _SyncIsVisible, value);
        }

        public ICommand SwitchCurrencyCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand NavigateToTransactionDetailsCommand { get; }

        public HomeViewModel()
        {
            _Logger = _WalletService.Logger;

            NavigateToTransactionDetailsCommand = new Command(NavigateToTransactionDetails);
            SwitchCurrencyCommand = new Command(SwitchCurrency);
            SearchCommand = new Command(StartSearch);

            PriceText = Constants.BTC_UNIT_LABEL_TMP;

            InitializeWalletServiceTransactions();
            InitializePrecioAndWalletTimers();
        }

        void InitializeWalletServiceTransactions()
        {
            if (_WalletService.IsStarted)
            {
                LoadTransactions();
                UpdateSyncingStatus();
                AddWalletServiceEvents();
            }
            else
            {
                _WalletService.OnStarted += _WalletService_OnStarted;
            }

            // FIXME for now we gonna include the unconfirmed transactions, but this should not be the case
            if (_WalletService.IsStarted)
            {
                Amount = _WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);
                
                _AmountBTC = Amount; // A copy of the Amount in BTC is saved to switch currencies quicker
                // and to not affect the original amount.
                
                if(Preferences.Get("currency", "BTC") != "BTC")
                {
                    Amount *= (decimal) _NewRate;
                }
            }
            else
            {
                Amount = 0.0m;

                _WalletService.OnStarted += _WalletService_OnStarted_ViewAppearing;
            }

            IsBtcEnabled = true;
        }

        void StartSearch()
        {
            _Logger.Debug("Search is not implemented yet!");
        }

        void UpdateSyncingStatus()
        {
            SyncIsVisible = !_WalletService.IsSyncedToTip();

            double progress = _WalletService.GetSyncedProgress();

            if (progress.Equals(0.00))
            {
                var chainTip = _WalletService.GetChainTip();

                _Logger.Debug("[{0}] chainTip => {1}", nameof(UpdateSyncingStatus), chainTip.Height);

                SyncDateText = Constants.SYNC_LOADING_HEADERS;
            }
            else
            {
                SyncDateText = $"{_WalletService.GetSyncedProgressPercentage()}% {_WalletService.GetLastSyncedDate()} ({_WalletService.GetLastSyncedBlockHeight()})";

            }
            SyncCurrentProgress = _WalletService.GetSyncedProgress();

            _Logger.Debug(
                "[{0}] IsSyncedToTip => {1}, GetLastSyncedDate => {2}, GetLastSyncedBlockHeight => {3}, GetSyncedProgressPercentage => {4}",
                nameof(UpdateSyncingStatus),
                _WalletService.IsSyncedToTip(),
                _WalletService.GetLastSyncedDate(),
                _WalletService.GetLastSyncedBlockHeight(),
                _WalletService.GetSyncedProgressPercentage()
            );
        }

        void SwitchCurrency()
        {
            var currency = Preferences.Get("currency", "BTC");
            if (currency == "BTC")
            {
                Preferences.Set("currency", "USD");
                IsBtcEnabled = false;
                Amount *= (decimal)_NewRate;
            }
            else
            {
                Preferences.Set("currency", "BTC");
                IsBtcEnabled = true;
                Amount = _AmountBTC;
            }
        }

        void InitializePrecioAndWalletTimers()
        {
            // Run and schedule next times precio will be called
            Task.Run(async () =>
            {
                // Gets first BTC-USD rate.
                var rate = (await _PrecioService.GetRates()).SingleOrDefault(r => r.Code == "USD");
                if (rate != null)
                {
                    // Sets both old and new rate for comparison on timer to optimize fiat currency updates based on current rate.
                    _OldRate = _NewRate = rate.Rate;

                    if (Preferences.Get("currency", "BTC") != null)
                    {
                        Amount *= (decimal)_NewRate;
                    }
                }
                return Task.FromResult(true);
            });

            Device.StartTimer(TimeSpan.FromSeconds(Constants.PRECIO_TIMER_INTERVAL), () =>
            {
                Task.Run(RatesAsync);

                if (Preferences.Get("currency", "BTC") != "BTC")
                {
                    Amount = _AmountBTC * (decimal)_NewRate;
                    
                    if (_OldRate != _NewRate)
                    {
                        _OldRate = _NewRate;
                        //TODO: Update transactions with new rate.
                        for (int i = 0; i < Transactions.Count; i++) // DO NOT convert this into a foreach loop or LINQ statement.
                        {
                            //TODO: Update currency
                        }
                    }
                }
                else
                {
                    // TODO: Convert Fiat to BTC
                }

                //TODO: WIP, will polish rate comparision.
                
                return true;
            });
        }

        private void _WalletService_OnStarted_ViewAppearing(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (_Lock)
                {
                    Amount = _WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);

                    _WalletService.OnStarted -= _WalletService_OnStarted_ViewAppearing;
                }
            });
        }

        public void ReScan()
        {
            var seedBirthday = DateTimeOffset.FromUnixTimeSeconds(SecureStorageService.GetSeedBirthday());

            _WalletService.ReScan(seedBirthday);
        }

        void NavigateToTransactionDetails()
        {
            if (CurrentTransaction == null) return;

            MessagingCenter.Send(this, "NavigateToTransactionDetail", CurrentTransaction as TransactionModel);

            CurrentTransaction = null;
        }

        async Task RatesAsync()
        {
            var rates = await _PrecioService.GetRates();

            foreach (var rate in rates)
            {
                if (rate.Code == "USD")
                {
                    var price = _NewRate = rate.Rate;
                    //PriceText = string.Format(CultureInfo.CurrentCulture, Constants.BTC_UNIT_LABEL, price);
                    PriceText = price.ToString();
                    break;
                }
            }
        }

        void WalletSyncManager_OnSyncProgressUpdate(object sender, WalletPositionUpdatedEventArgs e)
        {
            _Logger.Debug(
                "[{0}] e.NewPosition.Height => {1}, e.PreviousPosition.Height => {2}",
                nameof(WalletSyncManager_OnSyncProgressUpdate),
                e.NewPosition.Height,
                e.PreviousPosition.Height
            );

            UpdateSyncingStatus();
        }

        void _WalletService_OnStarted(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (_Lock)
                {
                    LoadTransactions();

                    UpdateSyncingStatus();

                    AddWalletServiceEvents();
                }
            });
        }

        void AddWalletServiceEvents()
        {
            _WalletService.WalletManager.OnNewTransaction += WalletManager_OnNewTransaction;
            _WalletService.WalletManager.OnNewSpendingTransaction += WalletManager_OnNewSpendingTransaction;
            _WalletService.WalletManager.OnUpdateTransaction += WalletManager_OnUpdateTransaction;
            _WalletService.WalletManager.OnUpdateSpendingTransaction += WalletManager_OnUpdateSpendingTransaction;

            _WalletService.WalletSyncManager.OnWalletPositionUpdate += WalletSyncManager_OnSyncProgressUpdate;
            _WalletService.WalletSyncManager.OnWalletSyncedToTipOfChain += WalletSyncManager_OnWalletSyncedToTipOfChain;
        }

        private void WalletSyncManager_OnWalletSyncedToTipOfChain(object sender, NBitcoin.ChainedBlock e)
        {
            UpdateSyncingStatus();

            SyncIsVisible = false;
        }

        void WalletManager_OnUpdateSpendingTransaction(object sender, TransactionData e)
        {
            UpdateTransactionsCollectionWith(e);
        }

        void WalletManager_OnUpdateTransaction(object sender, TransactionData e)
        {
            UpdateTransactionsCollectionWith(e);
        }

        void WalletManager_OnNewSpendingTransaction(object sender, TransactionData e)
        {
            UpdateTransactionsCollectionWith(e);
        }

        void WalletManager_OnNewTransaction(object sender, TransactionData e)
        {
            AddToTransactionsCollectionWith(e);
        }

        void AddToTransactionsCollectionWith(TransactionData txData)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (_Lock)
                {
                    // Double Check if the tx is there or not...
                    if (Transactions.Any(tx => tx.Id == txData.Id)) return;

                    Transactions.Insert(0, TransactionModel.FromTransactionData(txData));
                }
            });
        }

        void UpdateTransactionsCollectionWith(TransactionData txData)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (_Lock)
                {
                    var txModel = TransactionModel.FromTransactionData(txData);
                    var tx = Transactions.FirstOrDefault(tx1 => tx1.Id == txData.Id);

                    if (tx is null) return;
                    if (tx == txModel) return;

                    int index = Transactions.IndexOf(tx);

                    Transactions.Remove(tx);
                    Transactions.Insert(index, txModel);
                }
            });
        }

        void LoadTransactions()
        {
            var txs = _WalletService.GetCurrentAccountTransactions().OrderBy(
                (TransactionData txData) => txData.CreationTime
            );

            foreach (var tx in txs)
            {
                if (Transactions.Any(txModel => txModel.Id == tx.Id))
                {
                    var item = Transactions.FirstOrDefault(txModel => txModel.Id == tx.Id);
                    var newItem = TransactionModel.FromTransactionData(tx);

                    if (item == newItem) continue;

                    int index = Transactions.IndexOf(item);
                    Transactions.Remove(item);
                    Transactions.Insert(index, newItem);

                    continue;
                }

                Transactions.Insert(0, TransactionModel.FromTransactionData(tx));
            }
        }

        string GetAmountLabelText(TransactionData tx)
        {
            if (tx is null)
            {
                _Logger.Debug("Tx is null, because we need to figure out how to change amounts");

                return "";
            }

            var preferences = Preferences.Get("currency", "BTC");
            if (preferences == "BTC")
            {
                if (tx.IsSend == true)
                    return string.Format(Constants.SENT_AMOUNT, preferences, $"-{tx.AmountSent}");

                return string.Format(Constants.RECEIVE_AMOUNT, preferences, tx.Amount);
            }
            else
            {
                if (tx.IsSend == true)
                    return string.Format(
                        Constants.SENT_AMOUNT,
                        preferences,
                        $"-{tx.AmountSent.ToUsd((decimal)_NewRate):F2}");

                return string.Format(
                    Constants.RECEIVE_AMOUNT,
                    preferences,
                    $"{tx.Amount.ToUsd((decimal)_NewRate):F2}");
            }
        }
    }
}
