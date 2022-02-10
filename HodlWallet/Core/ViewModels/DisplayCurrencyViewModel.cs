//
// DisplayCurrencyViewModel.cs
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
using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Models;
using HodlWallet.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace HodlWallet.Core.ViewModels 
{
    class DisplayCurrencyViewModel : BaseViewModel
    {
        public ObservableCollection<CurrencySymbolEntity> currencySymbolEntities { get; set; } = new(); //Object reference not set to an instance of an object.

        public ObservableCollection<CurrencySymbolEntity> selectedCurrency { get; set; } = new();

        CurrencyEntity rate;
        public CurrencyEntity Rate
        {
            get => rate;
            set => SetProperty(ref rate, value, nameof(Rate));
        }

        public DisplayCurrencyViewModel()
        {
            Task.Run( PopulateCurrency );
        }

        public async Task PopulateCurrency()
        {
            IsLoading = true;
            try
            {
                var currencyEntities = await PrecioHttpService.GetRates();

                foreach (var currencyEntity in currencyEntities)
                {
                    currencySymbolEntities.Add(new CurrencySymbolEntity
                    {
                        Code = currencyEntity.Code,
                        Symbol = GetSymbol(currencyEntity.Code),
                        Name = currencyEntity.Name,
                        Rate = currencyEntity.Rate
                    });
                }
                IsLoading = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[PopulateCurrency] Exception on PopulateCurrencyt! => {e.Message}");
            }
        }

        private string GetSymbol(string code)
        {
            string currentSymbol = Constants.CURRENCY_SYMBOLS[Constants.EMPTY_CURRENCY_SYMBOL_KEY];

            string outSymbol;
            if (Constants.CURRENCY_SYMBOLS.TryGetValue(code, out outSymbol))
            {
                if (outSymbol.IndexOf("\\u") >= 0)
                {
                    try
                    {
                        currentSymbol = Char.Parse(outSymbol).ToString();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"[DecodeSymbol] Error while trying to parse a currency's Unicode => {e.Message}");
                    }
                }
                else
                    currentSymbol = outSymbol;
            }
            return currentSymbol;
        }

    }
}