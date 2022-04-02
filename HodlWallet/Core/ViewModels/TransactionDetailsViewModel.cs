//
// TransactionDetailsViewModel.cs
//
// Copyright (c) 2019 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;

using NBitcoin;

using Liviano.Models;

using HodlWallet.Core.Models;
using HodlWallet.Core.Utils;
using HodlWallet.UI.Locale;
using System.Diagnostics;

namespace HodlWallet.Core.ViewModels
{
    public class TransactionDetailsViewModel : BaseViewModel
    {
        string BlockExplorerAddressUri => WalletService.GetNetwork() == Network.Main ?
                Constants.BLOCK_EXPLORER_ADDRESS_MAINNET_URI :
                Constants.BLOCK_EXPLORER_ADDRESS_TESTNET_URI;

        string BlockExplorerTransactionsUri => WalletService.GetNetwork() == Network.Main ?
                Constants.BLOCK_EXPLORER_TRANSACTION_MAINNET_URI :
                Constants.BLOCK_EXPLORER_TRANSACTION_TESTNET_URI;

        public ICommand ShowFaqCommand { get; }

        public ICommand BrowseAddressCommand { get; }
        
        public ICommand BrowseTransactionIdCommand { get; }

        public string StatusTitle => LocaleResources.TransactionDetails_statusTitle;
        
        public string MemoTitle => LocaleResources.TransactionDetails_memoTitle;
        
        public string AmountTitle => LocaleResources.TransactionDetails_amountTitle;
        
        public string TransactionIdTitle => LocaleResources.TransactionDetails_transactionIdTitle;
        
        public string ConfirmedBlockTitle => LocaleResources.TransactionDetails_confirmedBlockTitle;

        TransactionModel transactionModel;
        public TransactionModel TransactionModel
        {
            get => transactionModel;
            set
            {
                SetProperty(ref transactionModel, value);

                GetTransactionModelData();
            }
        }

        DateTimeOffset createdAt;
        public DateTimeOffset CreatedAt
        {
            get => createdAt;
            set => SetProperty(ref createdAt, value);
        }

        string createdAtText;
        public string CreatedAtText
        {
            get => createdAtText;
            set => SetProperty(ref createdAtText, value);
        }

        Money amount;
        public Money Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        string amountText;
        public string AmountText
        {
            get => amountText;
            set => SetProperty(ref amountText, value);
        }

        string preposition;
        public string Preposition
        {
            get => preposition;
            set => SetProperty(ref preposition, value);
        }

        string address;
        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }

        string addressText;
        public string AddressText
        {
            get => addressText;
            set => SetProperty(ref addressText, value);
        }

        string addressTitle;
        public string AddressTitle
        {
            get => addressTitle;
            set => SetProperty(ref addressTitle, value);
        }

        string statusText;
        public string StatusText
        {
            get => statusText;
            set => SetProperty(ref statusText, value);
        }

        string memoText;
        public string MemoText
        {
            get => memoText;
            set
            {
                if (value != null)
                {
                    SetProperty(ref memoText, value);
                }
            }
        }

        string amountWithFeeText;
        public string AmountWithFeeText
        {
            get => amountWithFeeText;
            set => SetProperty(ref amountWithFeeText, value);
        }

        string startingBalanceText;
        public string StartingBalanceText
        {
            get => startingBalanceText;
            set => SetProperty(ref startingBalanceText, value);
        }

        string endingBalanceText;
        public string EndingBalanceText
        {
            get => endingBalanceText;
            set => SetProperty(ref endingBalanceText, value);
        }

        string idText;
        public string IdText
        {
            get => idText;
            set => SetProperty(ref idText, value);
        }

        uint256 id;
        public uint256 Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        string confirmedBlockText;
        public string ConfirmedBlockText
        {
            get => confirmedBlockText;
            set => SetProperty(ref confirmedBlockText, value);
        }

        public TransactionDetailsViewModel()
        {
            ShowFaqCommand = new Command(() => _ = ShowFaq());
            BrowseAddressCommand = new Command(() => _ = AddressToBrowser());
            BrowseTransactionIdCommand = new Command(() => _ = IdToBrowser());
        }

        public void StoreMemo()
        {
            var tx = new Tx(TransactionModel.TransactionData) { Memo = MemoText };
            //_WalletService.Wallet.UpdateCurrentTransaction(tx);
        }

        void GetTransactionModelData()
        {
            AddressText = transactionModel.AddressText;
            AddressTitle = transactionModel.AddressTitle;
            CreatedAtText = transactionModel.CreatedAtText;
            AmountText = transactionModel.AmountText;
            StatusText = transactionModel.StatusText;
            MemoText = transactionModel.MemoText;
            AmountWithFeeText = transactionModel.AmountWithFeeText;
            Address = transactionModel.Address;
            Id = transactionModel.Id;
            IdText = transactionModel.IdText;
            ConfirmedBlockText = transactionModel.ConfirmedBlockText;
            Preposition = transactionModel.Preposition;
        }

        Task ShowFaq()
        {
            return Task.FromResult(this);
        }

        async Task AddressToBrowser()
        {
            var uri = new Uri(string.Format(BlockExplorerAddressUri, Address));

            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not open Browser --> " + e); 
            }
        }

        async Task IdToBrowser()
        {
            var uri = new Uri(string.Format(BlockExplorerTransactionsUri, Id));

            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not open Browser --> " + e);
            }
        }
    }
}
