﻿//
// PriceView.xaml.cs
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
using System.Linq;

using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using Microcharts;

using HodlWallet2.Core.ViewModels;
using HodlWallet2.Core.Models;
using Microcharts.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class PriceView : ContentPage
    {
        PriceViewModel _ViewModel => (PriceViewModel)BindingContext;

        public PriceView()
        {
            InitializeComponent();

            SubscribeToMessages();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ViewModel.View_OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _ViewModel.View_OnDisappearing();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<PriceViewModel>(this, "DrawPricesChart", DrawPricesChart);
        }

        private void DrawPricesChart(PriceViewModel vm)
        {
            // TODO work on the Prices1h chart, right now it only show 1 dot!
            //var pricesList = new string[] { "Prices1h", "Prices1d", "Prices1w", "Prices1m", "Prices1y", "PricesAll" };
            var pricesList = new string[] { "Prices1d", "Prices1w", "Prices1m", "Prices1y", "PricesAll" };
            foreach (var key in pricesList)
            {
                var prices = (PricesEntity)vm.GetType().GetProperty(key).GetValue(vm);

                if (prices is null) continue;

                UpdateChartView(key, prices);
            }
        }

        void UpdateChartView(string key, PricesEntity prices)
        {
            var chartView = (ChartView)FindByName($"{key}ChartView");

            if (chartView is null) return;

            var chart = prices.GetLineChart();

            Device.BeginInvokeOnMainThread(() =>
            {
                chartView.Chart = chart;
            });
        }

        void Close_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
