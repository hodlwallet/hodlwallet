﻿//
// IPrecioService.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
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
using System.Collections.Generic;
using System.ComponentModel;

using HodlWallet.Core.Models;

namespace HodlWallet.Core.Interfaces
{
    public interface IPrecioService
    {
        bool IsStarted { get; }

        BtcPriceChangeEntity Btc1dChange { get; set; }
        BtcPriceChangeEntity Btc1hChange { get; set; }
        BtcPriceChangeEntity Btc1mChange { get; set; }
        BtcPriceChangeEntity Btc1wChange { get; set; }
        BtcPriceChangeEntity Btc1yChange { get; set; }
        BtcPriceChangeEntity BtcAllChange { get; set; }

        BtcPriceEntity BtcPrice { get; set; }

        List<List<object>> ExchangesLeaderboard { get; set; }

        MarketCapEntity MarketCap { get; set; }

        CurrencyEntity Rate { get; set; }

        PricesEntity Prices1d { get; set; }
        PricesEntity Prices1h { get; set; }
        PricesEntity Prices1m { get; set; }
        PricesEntity Prices1w { get; set; }
        PricesEntity Prices1y { get; set; }
        PricesEntity PricesAll { get; set; }

        List<CurrencyEntity> CurrencyEntities { get; set; }

        void StartHttpTimers();
        void Init();

        void GetRateCurrency();

        event PropertyChangedEventHandler PropertyChanged;
    }
}