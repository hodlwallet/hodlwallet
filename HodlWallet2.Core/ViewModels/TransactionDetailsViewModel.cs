using System;
using System.Threading.Tasks;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using HodlWallet2.Core.Models;

using HodlWallet2.Core.Interfaces;

namespace HodlWallet2.Core.ViewModels
{
    public class TransactionDetailsViewModel : BaseViewModel<Transaction>
    {
        readonly IWalletService _WalletService;
        Transaction _Transaction;

        public string TransactionDetailsTitle => "Transaction Details";
        public string StatusTitle => "Status";
        public string MemoTitle => "Memo";
        public string AmountTitle => "Amount";
        public string TransactionIdTitle => "Bitcoin Transaction ID";
        public string ConfirmedBlockTitle => "Confirmed in Block";

        string _DateText;
        public string DateText
        {
            get => _DateText;
            set => SetProperty(ref _DateText, value);
        }

        string _AmountText;
        public string AmountText
        {
            get => _AmountText;
            set => SetProperty(ref _AmountText, value);
        }

        string _AddressToFromText;
        public string AddressToFromText
        {
            get => _AddressToFromText;
            set => SetProperty(ref _AmountText, value);
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

        string _AddressText;
        public string AddressText
        {
            get => _AddressText;
            set => SetProperty(ref _AddressText, value);
        }

        string _TransactionIdText;
        public string TransactionIdText
        {
            get => _TransactionIdText;
            set => SetProperty(ref _TransactionIdText, value);
        }

        string _ConfirmedBlockText;
        public string ConfirmedBlockText
        {
            get => _ConfirmedBlockText;
            set => SetProperty(ref _ConfirmedBlockText, value);
        }

        public MvxAsyncCommand CloseCommand { get; }
        public MvxAsyncCommand ShowFaqCommand { get; }

        public TransactionDetailsViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService,
            IWalletService walletService) : base(logProvider, navigationService)
        {
            _WalletService = walletService;

            CloseCommand = new MvxAsyncCommand(Close);
            ShowFaqCommand = new MvxAsyncCommand(ShowFaq);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            DateText = _Transaction.DateAndTime;
            AmountText = _Transaction.Amount;
            AddressToFromText = _Transaction.AtAddress;
            StatusText = _Transaction.Confirmations;
            MemoText = _Transaction.Memo;
            AddressText = _Transaction.Address;
            TransactionIdText = _Transaction.Id;
            ConfirmedBlockText = _Transaction.BlockHeight?.ToString();

            /*  TODO: Add Setup for Amount Section
             */
        }

        public override void Prepare(Transaction parameter)
        {
            _Transaction = parameter;
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
    }
}
