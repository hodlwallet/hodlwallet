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

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

using Liviano;
using Liviano.Models;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.Utils;
using HodlWallet2.Core.Services;

namespace HodlWallet2.Core.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        Serilog.ILogger _Logger;

        readonly IWalletService _WalletService;
        readonly IPrecioService _PrecioService;

        public string SendText => "Send";
        public string ReceiveText => "Receive";
        public string MenuText => "Menu";
        public string SyncTitleText => "SYNCING";

        decimal _Amount;
        float _NewRate;
        float _OldRate;
        bool _IsBtcEnabled;
        object _CurrentTransaction;
        
        ObservableCollection<Transaction> _Transactions = new ObservableCollection<Transaction>();

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

        public ObservableCollection<Transaction> Transactions
        {
            get => _Transactions;
            set => SetProperty(ref _Transactions, value);
        }

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

        public MvxCommand NavigateToSendViewCommand { get; }
        public MvxCommand NavigateToReceiveViewCommand { get; }
        public MvxCommand NavigateToMenuViewCommand { get; }
        public MvxCommand SwitchCurrencyCommand { get; }
        public MvxCommand SearchCommand { get; }
        public MvxCommand NavigateToTransactionDetailsCommand { get; }

        public DashboardViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService,
            IPrecioService precioService) : base(logProvider, navigationService)
        {
            _WalletService = walletService;
            _Logger = _WalletService.Logger;
            _PrecioService = precioService;

            NavigateToSendViewCommand = new MvxCommand(NavigateToSendView);
            NavigateToReceiveViewCommand = new MvxCommand(NavigateToReceiveView);
            NavigateToMenuViewCommand = new MvxCommand(NavigateToMenuView);
            NavigateToTransactionDetailsCommand = new MvxCommand(NavigateToTransactionDetails);
            SwitchCurrencyCommand = new MvxCommand(SwitchCurrency);
            SearchCommand = new MvxCommand(StartSearch);

            PriceText = Constants.BTC_UNIT_LABEL_TMP;

            InitializeWalletServiceTransactions();
        }

        void InitializeWalletServiceTransactions()
        {
            if (_WalletService.IsStarted)
            {
                _WalletService_OnStarted(_WalletService, null);

                AddWalletServiceEvents();
            }
            else
            {
                _WalletService.OnStarted += _WalletService_OnStarted;
            }
        }

        void StartSearch()
        {
            // TODO, this is so far just to show and hide the syncbar randomly
            // since we're not gonna do search this is likely to be removed

            // Log what we supposed to show
            _Logger.Debug(!_WalletService.IsSyncedToTip() ? "[{0}] Show sync bar." : "[{0}] Hide sync bar.", nameof(StartSearch));

            UpdateSyncingStatus();

            // FIXME remove this...
            SyncIsVisible = true;
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
                Amount /= (decimal) _NewRate;
            }
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();

            // Run and schedule next times precio will be called
            Task.Run(async () =>
            {
                // Gets first BTC-USD rate.
                var rate = (await _PrecioService.GetRates()).SingleOrDefault(r => r.Code == "USD");
                if (rate != null)
                {
                    // Sets both old and new rate for comparison on timer to optimize fiat currency updates based on current rate.
                    _OldRate = _NewRate = rate.Rate;
                }
                return Task.FromResult(true);
            });
            
            Device.StartTimer(TimeSpan.FromSeconds(Constants.PRECIO_TIMER_INTERVAL), () =>
            {
                Task.Run(RatesAsync);
                //TODO: WIP, will polish rate comparision.
                if (_OldRate != _NewRate && Preferences.Get("currency", "BTC") != "BTC")
                {
                    _OldRate = _NewRate; 
                    //TODO: Update transactions with new rate.
                    foreach (var transaction in Transactions)
                    {
                        // This was intentionally left with null as placeholder. WARNING: IT'LL EXPLODE IF RUN.
                        // First of all, a view model should not have the responsibility to format the text for a label (this is UI's duty!).
                        // Second and very brief, this needs to be refactored (split) into two methods and the Transaction model needs to have two properties
                        // like Status and Amount (as float), this way it'll be flexible enough to update only one property based on current rate
                        // without having to convert numeric and string values to return a string(?) amount.
                        transaction.Amount = GetAmountLabelText(null);
                    }
                }
                return true;
            });

            // FIXME for now we gonna include the unconfirmed transactions, but this should not be the case	
            Amount = _WalletService.GetCurrentAccountBalanceInBTC(includeUnconfirmed: true);

            IsBtcEnabled = true;
        }

        public void ReScan()
        {
            var seedBirthday = DateTimeOffset.FromUnixTimeSeconds(SecureStorageProvider.GetSeedBirthday());

            _WalletService.ReScan(seedBirthday);
        }

        void NavigateToMenuView()
        {
            NavigationService.Navigate<MenuViewModel>();
        }

        void NavigateToReceiveView()
        {
            NavigationService.Navigate<ReceiveViewModel>();
        }

        void NavigateToSendView()
        {
            NavigationService.Navigate<SendViewModel>();
        }

        void NavigateToTransactionDetails()
        {
            if (CurrentTransaction == null) return;
            NavigationService.Navigate<TransactionDetailsViewModel, Transaction>(CurrentTransaction as Transaction);
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
                    PriceText = string.Format(CultureInfo.CurrentCulture, Constants.BTC_UNIT_LABEL, price);
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

        void _Transactions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO Handle change on individual.s
            // Different kind of changes that may have occurred in collection
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Code...
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                // Code...
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // Code...
            }
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                // Code...
            }
        }

        void _WalletService_OnStarted(object sender, EventArgs e)
        {
            // Try moving this initializer to the top.
            //_Transactions.CollectionChanged += _Transactions_CollectionChanged;

            LoadTransactions();

            UpdateSyncingStatus();

            AddWalletServiceEvents();
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
            // Double Check if the tx is there or not...
            for (int i = 0; i < _Transactions.Count; i++)
                if (_Transactions[i].Id == txData.Id.ToString()) return;

            _Transactions.Insert(0, CreateTransactionModelInstance(txData));

            RaisePropertyChanged(nameof(Transactions));

            foreach (var i in Enumerable.Range(0, 10)) _Logger.Information(new string('*', 60));

            _Logger.Information($"\n\nTX ADDED TO THE COLLECTION: {txData.Id.ToString()}\n\n");

            foreach (var i in Enumerable.Range(0, 10)) _Logger.Information(new string('*', 60));
        }

        void UpdateTransactionsCollectionWith(TransactionData txData)
        {
            for (int i = 0; i < _Transactions.Count; i++)
            {
                if (!_Transactions[i].Id.Equals(txData.Id.ToString())) continue;

                _Transactions[i] = CreateTransactionModelInstance(txData);

                _WalletService.Logger.Debug($"Updated tx: {txData.Id.ToString()}");

                RaisePropertyChanged(nameof(Transactions));

                foreach (var j in Enumerable.Range(0, 10)) Console.WriteLine(new string('*', 60));

                Console.WriteLine($"\n\nTX REPLACED IN THE COLLECTION: {txData.Id.ToString()}\n\n");

                foreach (var j in Enumerable.Range(0, 10)) Console.WriteLine(new string('*', 60));

            }
        }

        void LoadTransactions()
        {
            if (Transactions.Count != 0) return;

            var txs = _WalletService.GetCurrentAccountTransactions().OrderBy(
                (TransactionData txData) => txData.CreationTime
            ).Reverse();

            foreach (var tx in txs)
            {
                _Transactions.Add(CreateTransactionModelInstance(tx));
            }
        }
        
        IEnumerable<Transaction> CreateList(IEnumerable<TransactionData> txList)
        {
            var result = new List<Transaction>();

            foreach (var tx in txList)
            {
                result.Add(CreateTransactionModelInstance(tx));
                _WalletService.Logger.Information(JsonConvert.SerializeObject(tx, Formatting.Indented));
            }
            return result;
        }

        Transaction CreateTransactionModelInstance(TransactionData transactionData)
        {
            var tx = new Transaction {
                Id = transactionData.Id.ToString(),
                IsReceive = transactionData.IsReceive,
                IsSent = transactionData.IsSend,
                IsSpendable = transactionData.IsSpendable(),
                IsComfirmed = transactionData.IsConfirmed(),
                IsPropagated = transactionData.IsPropagated,
                BlockHeight = transactionData.BlockHeight,
                IsAvailable = transactionData.IsSpendable()
                        ? Constants.IS_AVAILABLE
                        : Constants.IS_NOT_AVAILABLE,
                /* TODO: Add Memo to Transaction Data
                   e.g.  Memo = transactionData.Memo, */
                Memo = "",
                Confirmations = "",
                Amount = GetAmountLabelText(transactionData),
                StatusColor = transactionData.IsSend == true
                        ? Color.FromHex(Constants.SYNC_GRADIENT_START_COLOR_HEX)
                        : Color.FromHex(Constants.GRAY_TEXT_TINT_COLOR_HEX),
                Address = _WalletService.GetAddressFromTransaction(transactionData),
                AtAddress = FormatAtAddressText(
                    transactionData.IsSend == true,
                    _WalletService.GetAddressFromTransaction(transactionData)),
                DateAndTime = transactionData.CreationTime.ToString(),
                Duration = DateTimeOffsetOperations.ShortDate(transactionData.CreationTime)
            };

            return tx;
        }

        string FormatAtAddressText(bool isSend, string address, bool chopAddress = false)
        {
            string fmtAddressText = "{0}: {1}";
            int addressPadding = 20;

            string preposition = isSend ? Constants.TO_LABEL : Constants.AT_LABEL;

            if (chopAddress)
            {
                address = string.Concat(
                    new string(address.Take(addressPadding).ToArray()),
                    "...",
                    new string(address.Reverse().Take(addressPadding).ToArray())
                );
            }

            return string.Format(fmtAddressText, preposition, address);
        }

        string GetAmountLabelText(TransactionData tx)
        {
            // FIXME BUG: Figure out why tx is null sometimes.
            if (tx is null)
            {
                return "";
            }

            var preferences = Preferences.Get("currency", "BTC"); 
            if (preferences == "BTC")
            {
                if (tx.IsSend == true)
                    return string.Format(Constants.SENT_AMOUNT, preferences, tx.Amount);

                return string.Format(Constants.RECEIVE_AMOUNT, preferences, tx.Amount);
            }
            else
            {
                if (tx.IsSend == true)
                    return string.Format(
                        Constants.SENT_AMOUNT, 
                        preferences, 
                        $"{tx.Amount.ToUsd((decimal)_NewRate):F2}");

                return string.Format(
                    Constants.RECEIVE_AMOUNT, 
                    preferences, 
                    $"{tx.Amount.ToUsd((decimal)_NewRate):F2}");
            }
        }
        
    }
}
