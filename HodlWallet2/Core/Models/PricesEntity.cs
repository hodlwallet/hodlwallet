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
using System.Runtime.InteropServices.ComTypes;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

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
        SKColor _Color => ((Color)Application.Current.Resources["TextSuccess"]).ToSKColor();
        SKColor _ChartBackgroundColor => ((Color)Application.Current.Resources["BackgroundPrimary"]).ToSKColor();

        [JsonProperty("meta")]
        public MetaEntity Meta { get; set; }

        [JsonProperty("prices")]
        public List<PriceEntity> Prices { get; set; }

        public LineChart GetLineChart()
        {
            return new LineChart
            {
                IsAnimated = false,
                PointMode = PointMode.None,
                LineMode = LineMode.Spline,
                LineAreaAlpha = 0,
                Entries = ToEntries(),
                MinValue = (float)Meta.Low.Price + 100.0f,
                MaxValue = (float)Meta.High.Price + 100.0f,
                BackgroundColor = _ChartBackgroundColor
            };
        }

        public IEnumerable<ChartEntry> ToEntries()
        {
            // Add first item
            yield return GetChartEntry(Prices[0], withValueLabel: true);

            if (Prices.Count <= 100)
            {
                for (int i = 1, count = Prices.Count - 2; i <= count; i++)
                    yield return GetChartEntry(Prices[i]);
            }
            else
            {
                int inc = Prices.Count > 500
                    ? 200
                    : 100;

                for (int i = inc, count = Prices.Count - 2; i <= count; i += inc)
                    yield return GetChartEntry(Prices[i]);
            }

            // Add last item
            yield return GetChartEntry(Prices[Prices.Count - 1], withValueLabel: true);
        }

        ChartEntry GetChartEntry(PriceEntity priceEntity, bool withValueLabel = false)
        {
            var price = priceEntity.Price;
            return new ChartEntry((float)price)
            {
                //Label = withValueLabel ? price.ToString("C") : null,
                Color = _Color,
                //ValueLabel = withValueLabel ? price.ToString("C") : null,
            };
        }
    }
}
