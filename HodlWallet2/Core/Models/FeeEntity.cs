//
// FeeEntity.cs
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
using Newtonsoft.Json;

namespace HodlWallet.Core.Models
{
    public class FeeEntity
    {
        [JsonProperty("fastest_sat_per_kilobyte")]
        public int FastestSatKB { get; set; }

        [JsonProperty("fastest_time_text")]
        public string FastestTime { get; set; }

        [JsonProperty("fastest_blocks")]
        public int FastestBlocks { get; set; }

        [JsonProperty("normal_sat_per_kilobyte")]
        public int NormalSatKB { get; set; }

        [JsonProperty("normal_time_text")]
        public string NormalTime { get; set; }

        [JsonProperty("normal_blocks")]
        public int NormalBlocks { get; set; }

        [JsonProperty("slow_sat_per_kilobyte")]
        public int SlowSatKB { get; set; }

        [JsonProperty("slow_time_text")]
        public string SlowTime { get; set; }

        [JsonProperty("slow_blocks")]
        public int SlowBlocks { get; set; }
    }
}
