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

using HodlWallet2.Core.ViewModels;

namespace HodlWallet2.UI.Views
{
    public partial class PriceView : ContentPage
    {
        PriceViewModel _ViewModel => (PriceViewModel)BindingContext;
        SKColor _ChartBackgroundColor => ((Color)Application.Current.Resources["BackgroundPrimary"]).ToSKColor();

        public PriceView()
        {
            InitializeComponent();

            SubscribeToMessages();
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
            var chart = new LineChart
            {
                IsAnimated = false,
                PointMode = PointMode.None,
                LineMode = LineMode.Spline,
                LineAreaAlpha = 0,
                Entries = _ViewModel.PricesList.ToEntries(),
                MinValue = (float)vm.PricesList.Meta.Low.Price + 100.0f,
                MaxValue = (float)vm.PricesList.Meta.High.Price + 100.0f,
                BackgroundColor = _ChartBackgroundColor
            };

            Device.BeginInvokeOnMainThread(() =>
            {
                PricesChartView.Chart = chart;
            });
        }

        void Close_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
