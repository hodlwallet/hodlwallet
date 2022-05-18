//
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
using System.Reactive.Linq;

using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet.Core.ViewModels;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrencySwitch : Frame
    {
        Color ActivatedColor => (Color)Application.Current.Resources["Fg"];
        Color DeactivatedColor => (Color)Application.Current.Resources["Fg5"];

        public CurrencySwitch()
        {
            InitializeComponent();
            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<CurrencySwitchViewModel>(this, nameof(ActivateBitcoin), ActivateBitcoin);
            MessagingCenter.Subscribe<CurrencySwitchViewModel>(this, nameof(ActivateFiat), ActivateFiat);
        }

        void ActivateFiat(CurrencySwitchViewModel _)
        {
            Observable.Start(() =>
            {
                bitcoinLabel.TextColor = DeactivatedColor;
                fiatLabel.TextColor = ActivatedColor;
            }, RxApp.MainThreadScheduler);
        }

        void ActivateBitcoin(CurrencySwitchViewModel _)
        {
            Observable.Start(() =>
            {
                bitcoinLabel.TextColor = ActivatedColor;
                fiatLabel.TextColor = DeactivatedColor;
            }, RxApp.MainThreadScheduler);
        }
    }
}