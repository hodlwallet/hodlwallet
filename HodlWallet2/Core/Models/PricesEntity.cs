//
// PricesEntity.cs
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
using System.Collections.Generic;

using Newtonsoft.Json;

using Microcharts;
using SkiaSharp;

namespace HodlWallet2.Core.Models
{
    public class MetaEntity
    {
        [JsonProperty("high")]
        public HighEntity High { get; set; }

        [JsonProperty("low")]
        public LowEntity Low { get; set; }
    }

    public class PriceEntity
    {
        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }
    }

    public class LowEntity : PriceEntity { }
    public class HighEntity : PriceEntity { }

    public class PricesEntity
    {
        [JsonProperty("meta")]
        public MetaEntity Meta { get; set; }

        [JsonProperty("prices")]
        public List<PriceEntity> Prices { get; set; }

        //public IEnumerable<ChartEntry> ToEntries()
        //{
        //    float initValue = 0.0f;
        //    foreach (PriceEntity price in Prices)
        //    {
        //        yield return new ChartEntry(initValue)
        //        {
        //            ValueLabel = initValue.ToString("C")
        //        };

        //        initValue += 1.0f;
        //    }
        //}
        public List<ChartEntry> ToEntries()
        {
            var chartEntries = new List<ChartEntry> { };

            foreach (PriceEntity price in Prices)
            {
                var entry = new ChartEntry((float)price.Price)
                {
                    Label = null,
                    Color = SKColor.Parse("#C89E26"),
                    ValueLabel = null
                };

                chartEntries.Add(entry);
            }

            return chartEntries;
        }
    }
}
