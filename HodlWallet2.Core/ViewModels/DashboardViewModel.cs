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

        ObservableCollection<Transaction> _Transactions;
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

            PriceText = Constants.BTC_UNIT_LABEL_TMP;
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
            }
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();

            // Run and schedule next times precio will be called
            Task.Run(RatesAsync);
            Device.StartTimer(TimeSpan.FromSeconds(Constants.PRECIO_TIMER_INTERVAL), () =>
            {
                Task.Run(() => RatesAsync());
                return true;
            });
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
                    var price = rate.Rate;
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

            _WalletService.Logger.Debug("Transactions loaded.");
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
            if (tx.IsSend == true)
                return string.Format(Constants.SENT_AMOUNT, tx.Amount);

            return string.Format(Constants.RECEIVE_AMOUNT, tx.Amount);
        }
    }
}