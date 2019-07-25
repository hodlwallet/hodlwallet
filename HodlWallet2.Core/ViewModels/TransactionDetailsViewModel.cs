using System;
using System.Threading.Tasks;

using Xamarin.Essentials;

using NBitcoin;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Interactions;
using HodlWallet2.Core.Utils;
using HodlWallet2.Core.Models;

namespace HodlWallet2.Core.ViewModels
{
    public class TransactionDetailsViewModel : BaseViewModel<TransactionModel>
    {
        readonly IWalletService _WalletService;
        TransactionModel _TransactionModel;

        MvxInteraction<DisplayAlertContent> _CopiedToClipboardInteraction = new MvxInteraction<DisplayAlertContent>();
        public IMvxInteraction<DisplayAlertContent> CopiedToClipboardInteraction => _CopiedToClipboardInteraction;

        public MvxAsyncCommand CloseCommand { get; }
        public MvxAsyncCommand ShowFaqCommand { get; }
        public IMvxAsyncCommand BrowseAddressCommand { get; }
        public IMvxAsyncCommand BrowseTransactionIdCommand { get; }

        public string TransactionDetailsTitle => "Transaction Details";
        public string StatusTitle => "Status";
        public string MemoTitle => "Memo";
        public string AmountTitle => "Amount";
        public string TransactionIdTitle => "Bitcoin Transaction ID";
        public string ConfirmedBlockTitle => "Confirmed in Block";

        DateTimeOffset _CreationTime;
        public DateTimeOffset CreationTime
        {
            get => _CreationTime;
            set => SetProperty(ref _CreationTime, value);
        }

        string _CreationTimeText;
        public string CreationTimeText
        {
            get => _CreationTimeText;
            set => SetProperty(ref _CreationTimeText, value);
        }

        Money _Amount;
        public Money Amount
        {
            get => _Amount;
            set => SetProperty(ref _Amount, value);
        }

        string _AmountText;
        public string AmountText
        {
            get => _AmountText;
            set => SetProperty(ref _AmountText, value);
        }

        string _Address;
        public string Address
        {
            get => _Address;
            set => SetProperty(ref _Address, value);
        }

        string _AddressText;
        public string AddressText
        {
            get => _AddressText;
            set => SetProperty(ref _AddressText, value);
        }

        string _AddressTitle;
        public string AddressTitle
        {
            get => _AddressTitle;
            set => SetProperty(ref _AddressTitle, value);
        }

        string _StatusText;
        public string StatusText
        {
            get => _StatusText;
            set => SetProperty(ref _StatusText, value);
        }

        string _MemoText;
        public string MemoText
        {
            get => _MemoText;
            set => SetProperty(ref _MemoText, value);
        }

        string _AmountWithFeeText;
        public string AmountWithFeeText
        {
            get => _AmountWithFeeText;
            set => SetProperty(ref _AmountWithFeeText, value);
        }

        string _StartingBalanceText;
        public string StartingBalanceText
        {
            get => _StartingBalanceText;
            set => SetProperty(ref _StartingBalanceText, value);
        }

        string _EndingBalanceText;
        public string EndingBalanceText
        {
            get => _EndingBalanceText;
            set => SetProperty(ref _EndingBalanceText, value);
        }

        string _IdText;
        public string IdText
        {
            get => _IdText;
            set => SetProperty(ref _IdText, value);
        }

        uint256 _Id;
        public uint256 Id
        {
            get => _Id;
            set => SetProperty(ref _Id, value);
        }

        string _ConfirmedBlockText;
        public string ConfirmedBlockText
        {
            get => _ConfirmedBlockText;
            set => SetProperty(ref _ConfirmedBlockText, value);
        }

        public TransactionDetailsViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService,
            IWalletService walletService) : base(logProvider, navigationService)
        {
            _WalletService = walletService;

            CloseCommand = new MvxAsyncCommand(Close);
            ShowFaqCommand = new MvxAsyncCommand(ShowFaq);
            BrowseAddressCommand = new MvxAsyncCommand(AddressToBrowser);
            BrowseTransactionIdCommand = new MvxAsyncCommand(IdToBrowser);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            GetTransactionModelData();
        }

        public override void Prepare(TransactionModel parameter)
        {
            _TransactionModel = parameter;
        }

        void GetTransactionModelData()
        {
            AddressText = _TransactionModel.AddressText;
            CreationTimeText = _TransactionModel.CreationTimeText;
            AmountText = _TransactionModel.AmountText;
            StatusText = _TransactionModel.StatusText;
            MemoText = _TransactionModel.MemoText;
            AmountWithFeeText = _TransactionModel.AmountWithFeeText;
            Address = _TransactionModel.Address;
            Id = _TransactionModel.Id;
            IdText = _TransactionModel.IdText;
            ConfirmedBlockText = _TransactionModel.ConfirmedBlockText;
        }

        Task ShowFaq()
        {
            //TODO: Implement FAQ
            return Task.FromResult(this);
        }

        async Task Close()
        {
            await NavigationService.Close(this);
        }

        async Task AddressToBrowser()
        {
            Uri uri;

            if (_WalletService.GetNetwork() == Network.Main)
            {
                uri = new Uri(string.Format(Constants.BLOCK_EXPLORER_ADDRESS_MAINNET_URI, Address));
            }
            else
            {
                uri = new Uri(string.Format(Constants.BLOCK_EXPLORER_ADDRESS_TESTNET_URI, Address));
            }

            await Browser.OpenAsync(uri, BrowserLaunchMode.External);
        }

        async Task IdToBrowser()
        {
            Uri uri;

            if (_WalletService.GetNetwork() == Network.Main)
            {
                uri = new Uri(string.Format(Constants.BLOCK_EXPLORER_TRANSACTION_MAINNET_URI, IdText));
            }
            else
            {
                uri = new Uri(string.Format(Constants.BLOCK_EXPLORER_TRANSACTION_TESTNET_URI, IdText));
            }

            await Browser.OpenAsync(uri, BrowserLaunchMode.External);
        }
    }
}
