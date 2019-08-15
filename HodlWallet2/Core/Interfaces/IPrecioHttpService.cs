//
// IPrecioHttpService.cs
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
using System.Collections.Generic;
using Refit;
using System.Threading.Tasks;
using HodlWallet2.Core.Models;

namespace HodlWallet2.Core.Interfaces
{
    public interface IPrecioHttpService
    {
        [Get("/hodl/rates.json")]
        Task<List<CurrencyEntity>> GetRates();

        [Get("/hodl.staging/rates.json")]
        Task<List<CurrencyEntity>> GetStagingRates();

        [Get("/hodl/fee-estimator.json")]
        Task<FeeEntity> GetFeeEstimator();

        [Get("/precio.json?period={period}")]
        Task<PricesEntity> GetPrecioByPeriod([AliasAs("period")] string period);
    }
}
