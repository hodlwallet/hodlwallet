using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

using Newtonsoft.Json;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Utils;
using Liviano;
using Liviano.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using NBitcoin;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Transaction = HodlWallet2.Core.Models.Transaction;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.Utils;

using Liviano.Models;

namespace HodlWallet2.Core.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        readonly IWalletService _WalletService;
        readonly IPrecioService _PrecioService;

        public string SendText => "Send";
        public string ReceiveText => "Receive";
        public string SyncText => "SYNCING";

        decimal _Amount;
        float _Rate;
        bool _IsBtcEnabled;
        
        ObservableCollection<Transaction> _Transactions;

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

        string _DateText;
        public string DateText
        {
            get => _DateText;
            set => SetProperty(ref _DateText, value);
        }

        double _Progress;
        public double Progress
        {
            get => _Progress;
            set => SetProperty(ref _Progress, value);
        }

        bool _IsVisible;
        public bool IsVisible
        {
            get => _IsVisible;
            set => SetProperty(ref _IsVisible, value);
        }

        public MvxCommand NavigateToSendViewCommand { get; }
        public MvxCommand NavigateToReceiveViewCommand { get; }
        public MvxCommand NavigateToMenuViewCommand { get; }
        public MvxCommand SwitchCurrencyCommand { get; }
        
        public DashboardViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService,
            IPrecioService precioService) : base(logProvider, navigationService)
        {
            _WalletService = walletService;
            _PrecioService = precioService;

            NavigateToSendViewCommand = new MvxCommand(NavigateToSendView);
            NavigateToReceiveViewCommand = new MvxCommand(NavigateToReceiveView);
            NavigateToMenuViewCommand = new MvxCommand(NavigateToMenuView);
            SwitchCurrencyCommand = new MvxCommand(SwitchCurrency);

            PriceText = Constants.BTC_UNIT_LABEL_TMP;
        }

        private void SwitchCurrency()
        {
            var currency = Preferences.Get("currency", "BTC");
            if (currency == "BTC")
            {
                Preferences.Set("currency", "USD");
                IsBtcEnabled = false;
                Amount *= (decimal)_Rate;
            }
            else
            {
                Preferences.Set("currency", "BTC");
                IsBtcEnabled = true;
                Amount /= (decimal) _Rate;
            }
            LoadTransactions();
        }

        private void NavigateToMenuView()
        {
            NavigationService.Navigate<MenuViewModel>();
        }

        private void NavigateToReceiveView()
        {
            NavigationService.Navigate<ReceiveViewModel>();
        }

        private void NavigateToSendView()
        {
            NavigationService.Navigate<SendViewModel>();
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            if (_WalletService.IsStarted)
            {
                _WalletService_OnStarted(_WalletService, null);
            }
            else
            {
                _WalletService.OnStarted += _WalletService_OnStarted; 
                //FIXME: Should the WalletService be started whenever the DashBoard view appears???, this will create problems on its own...
            }
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();

            // Run and schedule next times precio will be called
            Task.Run(RatesAsync);
            Device.StartTimer(TimeSpan.FromSeconds(Constants.PRECIO_TIMER_INTERVAL), () =>
            {
                Task.Run(RatesAsync);
                return true;
            });
            Amount = 1.0276M;
            IsBtcEnabled = true;
        }

        public void ReScan()
        {
            _WalletService.ReScan(new DateTimeOffset(new DateTime(2018, 12, 1)));
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

        async Task RatesAsync()
        {
            var rates = await _PrecioService.GetRates();

            foreach (var rate in rates)
            {
                if (rate.Code == "USD")
                {
                    var price = _Rate = rate.Rate;
                    PriceText = string.Format(CultureInfo.CurrentCulture, Constants.BTC_UNIT_LABEL, price);
                    break;
                }
            }
        }

        void WalletSyncManager_OnSyncProgressUpdate(object sender, WalletPositionUpdatedEventArgs e)
        {
            /* TODO: Update Progress During Sync
             *       Set 'IsVisible' to true when sync starts, false when complete.
             * e.g.  Progress = e.NewPosition.Height / _walletService.CurrentBlockHeight
             *       DateText = string.Format(CultureInfo.CurrentCulture, Constants.SyncDate, 
             *          e.NewPosition.GetMedianTimePast().UtcDateTime.ToShortDateString(), e.NewPosition.Height.ToString()); */
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

            _WalletService.WalletManager.OnNewTransaction += WalletManager_OnNewTransaction;
            _WalletService.WalletManager.OnNewSpendingTransaction += WalletManager_OnNewSpendingTransaction;
            _WalletService.WalletManager.OnUpdateTransaction += WalletManager_OnUpdateTransaction;
            _WalletService.WalletManager.OnUpdateSpendingTransaction += WalletManager_OnUpdateSpendingTransaction;

            _WalletService.WalletSyncManager.OnWalletPositionUpdate += WalletSyncManager_OnSyncProgressUpdate;

            LoadTransactionsIfEmpty();

            _Transactions.CollectionChanged += _Transactions_CollectionChanged;
        }

        void WalletManager_OnUpdateSpendingTransaction(object sender, TransactionData e)
        {
            LoadTransactionsIfEmpty();
            UpdateTransactionsCollectionWith(e);
        }

        void WalletManager_OnUpdateTransaction(object sender, TransactionData e)
        {
            LoadTransactionsIfEmpty();
            UpdateTransactionsCollectionWith(e);
        }

        void WalletManager_OnNewSpendingTransaction(object sender, TransactionData e)
        {
            LoadTransactionsIfEmpty();
            UpdateTransactionsCollectionWith(e);
        }

        void WalletManager_OnNewTransaction(object sender, TransactionData e)
        {
            LoadTransactionsIfEmpty();
            AddToTransactionsCollectionWith(e);
        }

        void AddToTransactionsCollectionWith(TransactionData txData)
        {
            // Double Check if the tx is there or not...
            for (int i = 0; i < _Transactions.Count; i++)
                if (_Transactions[i].Id == txData.Id.ToString()) return;

            _Transactions.Add(CreateTransactionModelInstance(txData));
        }

        void UpdateTransactionsCollectionWith(TransactionData txData)
        {
            for (int i = 0; i < _Transactions.Count; i++)
            {
                if (!_Transactions[i].Id.Equals(txData.Id.ToString())) continue;

                _Transactions[i] = CreateTransactionModelInstance(txData);

                _WalletService.Logger.Debug($"Updated tx: {txData.Id.ToString()}");
            }
        }

        void LoadTransactionsIfEmpty()
        {
            // Transactions are already loaded do something else!
            if (Transactions != null && Transactions.Count != 0) return;

            Transactions = new ObservableCollection<Transaction>(
                CreateList(
                    _WalletService.GetCurrentAccountTransactions().OrderBy(
                        (TransactionData txData) => txData.CreationTime
                    ).Reverse()
                )
            );
            _WalletService.Logger.Information(new string('*', 20));
        }

        public void ReScan()
        {
            _WalletService.ReScan(new DateTimeOffset(new DateTime(2018, 12, 1)));
            _WalletService.Logger.Debug("Transactions loaded.");
        }

        IEnumerable<Transaction> CreateList(IEnumerable<TransactionData> txList)
        {
            
            var result = new List<Transaction>();

            /*foreach (var tx in txList)
            {
                result.Add(new Transaction
                {
                    IsReceive = tx.IsReceive,
                    IsSent = tx.IsSend,
                    IsSpendable = tx.IsSpendable(),
                    IsComfirmed = tx.IsConfirmed(),
                    IsPropagated = tx.IsPropagated,
                    BlockHeight = tx.BlockHeight,
                    IsAvailable = tx.IsSpendable() ? Constants.IS_AVAILABLE : "",
                    Memo = Constants.MEMO_LABEL,
                    Status = GetStatus(tx),
                    StatusColor = tx.IsSend == true
                                    ? Color.FromHex(Constants.SYNC_GRADIENT_START_COLOR_HEX)
                                    : Color.FromHex(Constants.GRAY_TEXT_TINT_COLOR_HEX),
                    
                    AtAddress = WalletService.GetAddressFromTranscation(tx),
                    Duration = DateTimeOffsetOperations.ShortDate(tx.CreationTime)
                });
                result.Add(CreateTransactionModelInstance(tx));

                _WalletService.Logger.Information(JsonConvert.SerializeObject(tx, Formatting.Indented));
            }*/
            var tx = new TransactionData()
            {
                Amount = Money.Parse("1.0276")
            };
            var status = GetStatus(tx);
            result.Add(new Transaction
            {
                IsReceive = true,
                IsSent = false,
                IsSpendable = true,
                IsComfirmed = true,
                IsPropagated = true,
                BlockHeight = 1343,
                IsAvailable = "available",
                Memo = Constants.MEMO_LABEL,
                Status = status,
                StatusColor = true
                    ? Color.FromHex(Constants.SYNC_GRADIENT_START_COLOR_HEX)
                    : Color.FromHex(Constants.GRAY_TEXT_TINT_COLOR_HEX),
                /* TODO: Implement Send and Receive
                 * e.g   AtAddress = WalletService.GetAddressFromTranscation(tx), */
                Duration = DateTimeOffsetOperations.ShortDate(DateTimeOffset.Now)
            });
            result.Add(new Transaction
            {
                IsReceive = true,
                IsSent = false,
                IsSpendable = true,
                IsComfirmed = true,
                IsPropagated = true,
                BlockHeight = 1343,
                IsAvailable = "available",
                Memo = Constants.MEMO_LABEL,
                Status = status,
                StatusColor = true
                    ? Color.FromHex(Constants.SYNC_GRADIENT_START_COLOR_HEX)
                    : Color.FromHex(Constants.GRAY_TEXT_TINT_COLOR_HEX),
                /* TODO: Implement Send and Receive
                 * e.g   AtAddress = WalletService.GetAddressFromTranscation(tx), */
                Duration = DateTimeOffsetOperations.ShortDate(DateTimeOffset.Now)
            });
            result.Add(new Transaction
            {
                IsReceive = true,
                IsSent = false,
                IsSpendable = true,
                IsComfirmed = true,
                IsPropagated = true,
                BlockHeight = 1343,
                IsAvailable = "available",
                Memo = Constants.MEMO_LABEL,
                Status = status,
                StatusColor = true
                    ? Color.FromHex(Constants.SYNC_GRADIENT_START_COLOR_HEX)
                    : Color.FromHex(Constants.GRAY_TEXT_TINT_COLOR_HEX),
                /* TODO: Implement Send and Receive
                 * e.g   AtAddress = WalletService.GetAddressFromTranscation(tx), */
                Duration = DateTimeOffsetOperations.ShortDate(DateTimeOffset.Now)
            });
            result.Add(new Transaction
            {
                IsReceive = true,
                IsSent = false,
                IsSpendable = true,
                IsComfirmed = true,
                IsPropagated = true,
                BlockHeight = 1343,
                IsAvailable = "available",
                Memo = Constants.MEMO_LABEL,
                Status = status,
                StatusColor = true
                    ? Color.FromHex(Constants.SYNC_GRADIENT_START_COLOR_HEX)
                    : Color.FromHex(Constants.GRAY_TEXT_TINT_COLOR_HEX),
                /* TODO: Implement Send and Receive
                 * e.g   AtAddress = WalletService.GetAddressFromTranscation(tx), */
                Duration = DateTimeOffsetOperations.ShortDate(DateTimeOffset.Now)
            });
            result.Add(new Transaction
            {
                IsReceive = true,
                IsSent = false,
                IsSpendable = true,
                IsComfirmed = true,
                IsPropagated = true,
                BlockHeight = 1343,
                IsAvailable = "available",
                Memo = Constants.MEMO_LABEL,
                Status = status,
                StatusColor = true
                    ? Color.FromHex(Constants.SYNC_GRADIENT_START_COLOR_HEX)
                    : Color.FromHex(Constants.GRAY_TEXT_TINT_COLOR_HEX),
                /* TODO: Implement Send and Receive
                 * e.g   AtAddress = WalletService.GetAddressFromTranscation(tx), */
                Duration = DateTimeOffsetOperations.ShortDate(DateTimeOffset.Now)
            });
            result.Add(new Transaction
            {
                IsReceive = true,
                IsSent = false,
                IsSpendable = true,
                IsComfirmed = true,
                IsPropagated = true,
                BlockHeight = 1343,
                IsAvailable = "available",
                Memo = Constants.MEMO_LABEL,
                Status = status,
                StatusColor = true
                    ? Color.FromHex(Constants.SYNC_GRADIENT_START_COLOR_HEX)
                    : Color.FromHex(Constants.GRAY_TEXT_TINT_COLOR_HEX),
                /* TODO: Implement Send and Receive
                 * e.g   AtAddress = WalletService.GetAddressFromTranscation(tx), */
                Duration = DateTimeOffsetOperations.ShortDate(DateTimeOffset.Now)
            });
            
            return result;
        }

        Transaction CreateTransactionModelInstance(TransactionData transactionData)
        {
            return new Transaction {
                IsReceive = transactionData.IsReceive,
                IsSent = transactionData.IsSend,
                IsSpendable = transactionData.IsSpendable(),
                IsComfirmed = transactionData.IsConfirmed(),
                IsPropagated = transactionData.IsPropagated,
                BlockHeight = transactionData.BlockHeight,
                IsAvailable = transactionData.IsSpendable()
                        ? Constants.IS_AVAILABLE
                        : Constants.IS_NOT_AVAILABLE,
                Memo = transactionData.Memo,
                Amount = GetAmountLabelText(transactionData),
                StatusColor = transactionData.IsSend == true
                        ? Color.FromHex(Constants.SYNC_GRADIENT_START_COLOR_HEX)
                        : Color.FromHex(Constants.GRAY_TEXT_TINT_COLOR_HEX),
                AtAddress = FormatAtAddressText(
                    transactionData.IsSend == true,
                    _WalletService.GetAddressFromTransaction(transactionData)),
                Duration = DateTimeOffsetOperations.ShortDate(transactionData.CreationTime)
            };
        }

        string FormatAtAddressText(bool isSend, string address)
        {
            string fmtAddressText = "{0}: {1}";
            int addressPadding = 12;

            string preposition = isSend ? Constants.TO_LABEL : Constants.AT_LABEL;
            string choppedAddress = string.Concat(
                new string(address.Take(addressPadding).ToArray()),
                "...",
                new string(address.Reverse().Take(addressPadding).ToArray())
            );

            return string.Format(fmtAddressText, preposition, choppedAddress);
        }

        string GetAmountLabelText(TransactionData tx)
        {
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
                        $"{tx.Amount.ToUsd((decimal)_Rate):F2}");

                return string.Format(
                    Constants.RECEIVE_AMOUNT, 
                    preferences, 
                    $"{tx.Amount.ToUsd((decimal)_Rate):F2}");
            }
        }
    }
}