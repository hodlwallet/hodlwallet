//
// PriceViewModel.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 
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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HodlWallet2.Core.Models;
using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class PriceViewModel : BaseViewModel
    {
        string _PriceText;
        public string PriceText
        {
            get => _PriceText;
            set => SetProperty(ref _PriceText, value);
        }

        decimal _Price;
        public decimal Price
        {
            get => _Price;
            set => SetProperty(ref _Price, value);
        }

        PricesEntity _PricesList;
        public PricesEntity PricesList
        {
            get => _PricesList;
            set => SetProperty(ref _PricesList, value);
        }

        public PriceViewModel()
        {
            Title = "Price";

            IsLoading = true;

            Task.Run(Initialize);
        }

        void Initialize()
        {
            UpdatePrice();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                UpdatePrice();

                Debug.WriteLine($"[Initialize][StartTimerCallback] Timer new price is: {_PriceText}");

                return true;
            });

            IsLoading = false;
        }

        void UpdatePrice()
        {
            // TODO For now hardcoded to usd price.
            var price = _PrecioService.GetRates().Result.SingleOrDefault(r => r.Code == "USD");

            if (price != null)
            {
                Price = (decimal)price.Rate;
                PriceText = _Price.ToString("C");
            }
            else
            {
                Debug.WriteLine("[UpdatePrice] Unable to get price");
            }

            var prices = _PrecioService.GetPrices().Result;

            if (prices != null)
            {
                PricesList = prices;
            }
            else
            {
                Debug.WriteLine("[UpdatePrice] Unable to get prices");
            }
        }
    }
}
