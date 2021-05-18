//
// BtcPriceEntity.cs
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
using Newtonsoft.Json;

namespace HodlWallet.Core.Models
{
    public class BtcPriceEntity
    {
        // Lowest price in interval
        [JsonProperty("L")]
        public string L { get; set; }

        // Open price in interval
        [JsonProperty("O")]
        public string O { get; set; }

        // Last price time
        [JsonProperty("T")]
        public int T { get; set; }

        // Highest price in interval
        [JsonProperty("H")]
        public string H { get; set; }

        // Current price unformatted
        [JsonProperty("C_RAW")]
        public string CRaw { get; set; }

        // Current price formatted
        [JsonProperty("C")]
        public string C { get; set; }

        // Bitcoin Exchange of the price source
        [JsonProperty("EX")]
        public string Ex { get; set; }
    }
}
