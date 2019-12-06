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

using HodlWallet2.Core.Models;
using HodlWallet2.Core.Utils;
using HodlWallet2.UI.Locale;

namespace HodlWallet2.Core.ViewModels
{
    public class TransactionDetailsViewModel : BaseViewModel
    {
        public ICommand ShowFaqCommand { get; }
        public ICommand BrowseAddressCommand { get; }
        public ICommand BrowseTransactionIdCommand { get; }

        public string StatusTitle => LocaleResources.TransactionDetails_statusTitle;
        public string MemoTitle => LocaleResources.TransactionDetails_memoTitle;
        public string AmountTitle => LocaleResources.TransactionDetails_amountTitle;
        public string TransactionIdTitle => LocaleResources.TransactionDetails_transactionIdTitle;
        public string ConfirmedBlockTitle => LocaleResources.TransactionDetails_confirmedBlockTitle;

        TransactionModel _TransactionModel;
        public TransactionModel TransactionModel
        {
            get => _TransactionModel;
            set
            {
                SetProperty(ref _TransactionModel, value);

                GetTransactionModelData();
            }
        }

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
            set
            {
                if (value != null)
                {
                    SetProperty(ref _MemoText, value);
                }
            }
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

        public TransactionDetailsViewModel()
        {
            ShowFaqCommand = new Command(() => _ = ShowFaq());
            BrowseAddressCommand = new Command(() => _ = AddressToBrowser());
            BrowseTransactionIdCommand = new Command(() => _ = IdToBrowser());
        }

        public void StoreMemo()
        {
            var tx = new Tx(TransactionModel.TransactionData) { Memo = MemoText };
            _WalletService.Wallet.UpdateCurrentTransaction(tx);
        }

        void GetTransactionModelData()
        {
            AddressText = _TransactionModel.AddressText;
            AddressTitle = _TransactionModel.AddressTitle;
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
            return Task.FromResult(this);
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
