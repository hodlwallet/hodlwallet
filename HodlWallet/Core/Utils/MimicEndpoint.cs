//
// MimicEndpoint.cs
//
// Author:
//       Marconi Poveda <marconi@hodlwallet.com>
//
// Copyright (c) 2022 HODL Wallet
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

using Liviano.Services.Models;

namespace HodlWallet.Core.Utils
{
    public class MimicEndpoint
    {
        public List<CurrencyEntity> Currency { get; set; } = new();
        public MimicEndpoint()
        {
            Currency.Add(new CurrencyEntity { Name = "US Dollar", Code = "USD", Rate = 54469.82f });
            Currency.Add(new CurrencyEntity { Name = "Australian Dollar", Code = "AUD", Rate = 1.55f });
            Currency.Add(new CurrencyEntity { Name = "Albanian Lek", Code = "ALL", Rate = 1.25f });
            Currency.Add(new CurrencyEntity { Name = "Algerian Dinar", Code = "DZD", Rate = 1.2f });
            Currency.Add(new CurrencyEntity { Name = "Argentine Peso", Code = "ARS", Rate = 3.05f });
            Currency.Add(new CurrencyEntity { Name = "Afghan Afghani", Code = "AFN", Rate = 4.05f });
            Currency.Add(new CurrencyEntity { Name = "Angolan Kwanza", Code = "AOA", Rate = 8.05f });
            Currency.Add(new CurrencyEntity { Name = "Armenian Dram", Code = "AMD", Rate = 2.05f });
            Currency.Add(new CurrencyEntity { Name = "Azerbaijani Manat", Code = "AZN", Rate = 6.03f });
        }
    }
}