﻿//
// CurrencySwitch.xaml.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet.Core.ViewModels;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrencySwitch : StackLayout
    {
        Color ActivatedColor => (Color)Application.Current.Resources["Fg"];
        Color DeactivatedColor => (Color)Application.Current.Resources["Fg2"];
        string Bold => (string)Application.Current.Resources["Sans-Bold"];
        string Regular => (string)Application.Current.Resources["Sans-Regular"];

        public CurrencySwitch()
        {
            InitializeComponent();
            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<CurrencySwitchViewModel>(this, "ActivateBitcoin", ActivateBitcoin);
            MessagingCenter.Subscribe<CurrencySwitchViewModel>(this, "ActivateFiat", ActivateFiat);
        }

        void ActivateFiat(CurrencySwitchViewModel _)
        {
            bitcoinLabel.TextColor = DeactivatedColor;
            bitcoinLabel.FontFamily = Regular;

            fiatLabel.TextColor = ActivatedColor;
            fiatLabel.FontFamily = Bold;
        }

        void ActivateBitcoin(CurrencySwitchViewModel _)
        {
            bitcoinLabel.TextColor = ActivatedColor;
            bitcoinLabel.FontFamily = Bold;

            fiatLabel.TextColor = DeactivatedColor;
            fiatLabel.FontFamily = Regular;
        }
    }
}