//
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

using HodlWallet.Core.ViewModels;
using HodlWallet.Core.Models;
using Microcharts.Forms;

namespace HodlWallet.UI.Views
{
    public partial class PriceView : ContentPage
    {
        string currentPeriod = "1d";
        readonly Color increaseColor = (Color)Application.Current.Resources["FgGreen"];
        readonly Color decreaseColor = (Color)Application.Current.Resources["FgRed"];

        PriceViewModel ViewModel => (PriceViewModel)BindingContext;

        public PriceView()
        {
            InitializeComponent();

            SubscribeToMessages();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.View_OnAppearing();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<PriceViewModel>(this, "DrawPricesChart", DrawPricesChart);
            MessagingCenter.Subscribe<PriceViewModel, ValueTuple<BtcPriceChangeEntity, string>>(this, "UpdatePriceChangeWith", UpdatePriceChangeWith);
            MessagingCenter.Subscribe<PriceViewModel, string>(this, "ColorChangeLabelFor", ColorChangeLabelFor);
        }

        void ColorChangeLabelFor(PriceViewModel _, string polarity)
        {
            if (polarity == "+")
            {
                PriceChangeLabel.TextColor = increaseColor;
            }
            else
            {
                PriceChangeLabel.TextColor = decreaseColor;
            }
        }

        void DrawPricesChart(PriceViewModel vm)
        {
            // TODO work on the Prices1h and PricesAll chart, right now it only show 1 dot!
            //var pricesList = new string[] { "Prices1h", "Prices1d", "Prices1w", "Prices1m", "Prices1y", "PricesAll" };
            //var pricesList = new string[] { "Prices1d", "Prices1w", "Prices1m", "Prices1y", "PricesAll" };
            var pricesList = new string[] { "Prices1d", "Prices1w", "Prices1m", "Prices1y" };
            foreach (var key in pricesList)
            {
                var prices = (PricesEntity)vm.GetType().GetProperty(key).GetValue(vm);

                if (prices is null) continue;

                UpdateChartView(key, prices);

            }

            Device.BeginInvokeOnMainThread(() =>
            {
                GoToChartButtonsLayout.IsVisible = true;
                EnableGoToChartButton(GoTo1dChartButton);
                currentPeriod = "1d";
            });
        }

        void UpdatePriceChangeWith(PriceViewModel vm, ValueTuple<BtcPriceChangeEntity, string> values)
        {
            BtcPriceChangeEntity btcChange = values.Item1;
            string period = values.Item2;

            if (period != currentPeriod) return;

            vm.UpdatePriceChangeWithBtcChange(btcChange);

            if (btcChange.Type == "increase")
            {
                PriceChangeLabel.TextColor = increaseColor;
            }
            else
            {
                PriceChangeLabel.TextColor = decreaseColor;
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

        void GoTo1dChartButton_Clicked(object sender, EventArgs e)
        {
            ScrollToChartView(Prices1dChartView);

            var button = (Button)sender;
            EnableGoToChartButton(button);
            DisableGoToChartButtonsBut(button);
            ViewModel.UpdatePriceChangeWithPeriod("1d");
        }

        void GoTo1wChartButton_Clicked(object sender, EventArgs e)
        {
            ScrollToChartView(Prices1wChartView);

            var button = (Button)sender;
            EnableGoToChartButton(button);
            DisableGoToChartButtonsBut(button);
            ViewModel.UpdatePriceChangeWithPeriod("1w");
        }

        void GoTo1mChartButton_Clicked(object sender, EventArgs e)
        {
            ScrollToChartView(Prices1mChartView);

            var button = (Button)sender;
            EnableGoToChartButton(button);
            DisableGoToChartButtonsBut(button);
            ViewModel.UpdatePriceChangeWithPeriod("1m");
        }

        void GoTo1yChartButton_Clicked(object sender, EventArgs e)
        {
            ScrollToChartView(Prices1yChartView);

            var button = (Button)sender;
            EnableGoToChartButton(button);
            DisableGoToChartButtonsBut(button);
            ViewModel.UpdatePriceChangeWithPeriod("1y");
        }

        void GoToAllChartButton_Clicked(object sender, EventArgs e)
        {
            //ScrollToChartView(PricesAllChartView);

            var button = (Button)sender;
            EnableGoToChartButton(button);
            DisableGoToChartButtonsBut(button);
            ViewModel.UpdatePriceChangeWithPeriod("All");
        }

        void EnableGoToChartButton(Button goToChartButton)
        {
            goToChartButton.Style = (Style)Application.Current.Resources["PriceChartSwitchButtonEnabled"];
        }

        void DisableGoToChartButtonsBut(Button goToChartButton)
        {
            // TODO figure out why the all chart doens't work
            //foreach (var key in new string[] { "1d", "1w", "1m", "1y", "All" })
            foreach (var key in new string[] { "1d", "1w", "1m", "1y" })
            {
                var elemKey = $"GoTo{key}ChartButton";
                var button = (Button)FindByName(elemKey);

                if (button.Id == goToChartButton.Id) continue;

                button.Style = (Style)Application.Current.Resources["PriceChartSwitchButtonDisabled"];
            }
        }

        void ScrollToChartView(ChartView chartView)
        {
            PricesChartsScrollView.ScrollToAsync(
                chartView.X,
                PricesChartsScrollView.Y,
                animated: true);
        }
    }
}
