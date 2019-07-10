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
