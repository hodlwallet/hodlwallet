using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.Utils;
using Liviano.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        readonly IWalletService _WalletService;
        readonly IPrecioService _PrecioService;

        public string SendText => "Send";
        public string ReceiveText => "Receive";
        public string SyncText => "SYNCING";

        private ObservableCollection<Transaction> _Transactions;

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
            }
        }

        private void _WalletService_OnStarted(object sender, EventArgs e)
        {
            SubscribeToWalletEvents();
            LoadTransactions();
        }

        private void SubscribeToWalletEvents()
        {
            _WalletService.WalletManager.OnNewTransaction += WalletManager_OnWhateverTransaction;
            _WalletService.WalletManager.OnNewSpendingTransaction += WalletManager_OnWhateverTransaction;
            _WalletService.WalletManager.OnUpdateTransaction += WalletManager_OnWhateverTransaction;
            _WalletService.WalletSyncManager.OnWalletPositionUpdate += WalletSyncManager_OnSyncProgressUpdate;

        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();

            Device.StartTimer(TimeSpan.FromSeconds(Constants.PRECIO_TIMER_INTERVAL), () =>
            {
                Task.Run(() => RatesAsync());
                return true;
            });
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

        void WalletSyncManager_OnSyncProgressUpdate(object sender, WalletPositionUpdatedEventArgs e)
        {
            /* TODO: Update Progress During Sync
             *       Set 'IsVisible' to true when sync starts, false when complete.
             * e.g.  Progress = e.NewPosition.Height / _walletService.CurrentBlockHeight
             *       DateText = string.Format(CultureInfo.CurrentCulture, Constants.SyncDate, 
             *          e.NewPosition.GetMedianTimePast().UtcDateTime.ToShortDateString(), e.NewPosition.Height.ToString()); */
        }

        public void LoadTransactions()
        {
            Transactions = new ObservableCollection<Transaction>(
                CreateList(
                    _WalletService.GetCurrentAccountTransactions().OrderBy(
                        (TransactionData txData) => txData.CreationTime
                    )
                )
            );

            _WalletService.Logger.Information(new string('*', 20));
        }

        public void ReScan()
        {
            _WalletService.ReScan(new DateTimeOffset(new DateTime(2018, 12, 1)));
        }

        public IEnumerable<Transaction> CreateList(IEnumerable<TransactionData> txList)
        {
            var result = new List<Transaction>();

            foreach (var tx in txList)
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
                    /* TODO: Implement Send and Receive
                     * e.g   AtAddress = WalletService.GetAddressFromTranscation(tx), */
                    Duration = DateTimeOffsetOperations.ShortDate(tx.CreationTime)
                });

                _WalletService.Logger.Information(JsonConvert.SerializeObject(tx, Formatting.Indented));
            }
            return result;
        }

        private string GetStatus(TransactionData tx)
        {
            if (tx.IsSend == true)
                return string.Format(Constants.SENT_AMOUNT, tx.Amount);

            return string.Format(Constants.RECEIVE_AMOUNT, tx.Amount);
        }
    }
}