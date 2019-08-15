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

using Xamarin.Forms;

using HodlWallet2.Core.Models;
using HodlWallet2.Core.Services;
using Newtonsoft.Json;
using NBitcoin.Protocol;

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

        string _MarketcapText;
        public string MarketcapText
        {
            get => _MarketcapText;
            set => SetProperty(ref _MarketcapText, value);
        }

        string _PriceChangeText;
        public string PriceChangeText
        {
            get => _PriceChangeText;
            set => SetProperty(ref _PriceChangeText, value);
        }

        PricesEntity _Prices1h;
        public PricesEntity Prices1h
        {
            get => _Prices1h;
            set => SetProperty(ref _Prices1h, value);
        }

        PricesEntity _Prices1d;
        public PricesEntity Prices1d
        {
            get => _Prices1d;
            set => SetProperty(ref _Prices1d, value);
        }

        PricesEntity _Prices1w;
        public PricesEntity Prices1w
        {
            get => _Prices1w;
            set => SetProperty(ref _Prices1w, value);
        }

        PricesEntity _Prices1m;
        public PricesEntity Prices1m
        {
            get => _Prices1m;
            set => SetProperty(ref _Prices1m, value);
        }

        PricesEntity _Prices1y;
        public PricesEntity Prices1y
        {
            get => _Prices1y;
            set => SetProperty(ref _Prices1y, value);
        }

        PricesEntity _PricesAll;
        public PricesEntity PricesAll
        {
            get => _PricesAll;
            set => SetProperty(ref _PricesAll, value);
        }

        public PriceViewModel()
        {
            Title = "Price";

            IsLoading = true;
        }

        public void UpdatePriceChangeWithBtcChange(BtcPriceChangeEntity btcPriceChange)
        {
            string increase = btcPriceChange.Type == "increase" ? "+" : "-";
            string valueUsd = btcPriceChange.UsdChange;
            string valuePct = btcPriceChange.PctChange;

            PriceChangeText = $"{increase} {valueUsd} ({valuePct})";
        }

        public void UpdatePriceChangeWithPeriod(string periodKey)
        {
            var key = $"Btc{periodKey}Change";
            var btcPriceChange = (BtcPriceChangeEntity)_PrecioService.GetType().GetProperty(key).GetValue(_PrecioService);

            if (btcPriceChange is null) return;

            string polarity = btcPriceChange.Type == "increase" ? "+" : "-";
            string valueUsd = btcPriceChange.UsdChange;
            string valuePct = btcPriceChange.PctChange;

            PriceChangeText = $"{polarity} {valueUsd} ({valuePct})";

            MessagingCenter.Send(this, "ColorChangeLabelFor", polarity);
        }

        internal void View_OnAppearing()
        {
            Task.Run(Initialize);
        }

        void Initialize()
        {
            UpdatePrice();
            UpdateMarketcap();
            UpdatePriceChange1d();
            UpdateChartData();

            IsLoading = false;

            SubscribeToPrecioServiceMessages();
        }

        void SubscribeToPrecioServiceMessages()
        {
            MessagingCenter.Subscribe<PrecioService>(this, "BtcPriceChanged", PrecioService_BtcPriceChanged);
            MessagingCenter.Subscribe<PrecioService>(this, "MarketCapChanged", PrecioService_MarketCapChanged);
            MessagingCenter.Subscribe<PrecioService>(this, "Btc1hChangeChanged", PrecioService_Btc1hChangeChanged);
            MessagingCenter.Subscribe<PrecioService>(this, "Btc1dChangeChanged", PrecioService_Btc1dChangeChanged);
            MessagingCenter.Subscribe<PrecioService>(this, "Btc1wChangeChanged", PrecioService_Btc1wChangeChanged);
            MessagingCenter.Subscribe<PrecioService>(this, "Btc1mChangeChanged", PrecioService_Btc1mChangeChanged);
            MessagingCenter.Subscribe<PrecioService>(this, "Btc1yChangeChanged", PrecioService_Btc1yChangeChanged);
            MessagingCenter.Subscribe<PrecioService>(this, "BtcAllChangeChanged", PrecioService_BtcAllChangeChanged);
        }

        void UpdatePriceChange1d()
        {
            var priceChange = _PrecioService.Btc1dChange;

            if (priceChange is null) return;

            UpdatePriceChangeWithBtcChange(priceChange);
        }

        void PrecioService_BtcPriceChanged(PrecioService _)
        {
            UpdatePrice();
        }

        void PrecioService_MarketCapChanged(PrecioService _)
        {
            UpdateMarketcap();
        }

        void PrecioService_Btc1hChangeChanged(PrecioService ps)
        {
            UpdateChangedWith(ps.Btc1hChange, "1h");
        }

        void PrecioService_Btc1dChangeChanged(PrecioService ps)
        {
            UpdateChangedWith(ps.Btc1dChange, "1d");
        }

        void PrecioService_Btc1wChangeChanged(PrecioService ps)
        {
            UpdateChangedWith(ps.Btc1wChange, "1w");
        }

        void PrecioService_Btc1mChangeChanged(PrecioService ps)
        {
            UpdateChangedWith(ps.Btc1mChange, "1m");
        }

        void PrecioService_Btc1yChangeChanged(PrecioService ps)
        {
            UpdateChangedWith(ps.Btc1yChange, "1y");
        }

        void PrecioService_BtcAllChangeChanged(PrecioService ps)
        {
            UpdateChangedWith(ps.BtcAllChange, "All");
        }

        void UpdateChangedWith(BtcPriceChangeEntity btcPriceChange, string period)
        {
            MessagingCenter.Send(this, "UpdatePriceChangeWith", (btcPriceChange, period));
        }

        void UpdatePrice()
        {
            Price = decimal.Parse(_PrecioService.BtcPrice.CRaw);
            PriceText = Price.ToString("C");
        }

        void UpdateMarketcap()
        {
            MarketcapText = _PrecioService.MarketCap.Marketcap;
        }

        void UpdateChartData()
        {
            Prices1h = _PrecioService.Prices1h;
            Prices1d = _PrecioService.Prices1d;
            Prices1w = _PrecioService.Prices1w;
            Prices1m = _PrecioService.Prices1m;
            Prices1y = _PrecioService.Prices1y;
            PricesAll = _PrecioService.PricesAll;

            DrawPricesChart();
        }

        void DrawPricesChart()
        {
            MessagingCenter.Send(this, "DrawPricesChart");
        }
    }
}
