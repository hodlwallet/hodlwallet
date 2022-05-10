//
// PinPadDigits.xaml.cs
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PinPadDigits
    {
        Color DigitOnColor => (Color)Application.Current.Resources["FgSuccess"];

        Color DigitOffColor => (Color)Application.Current.Resources["Fg5"];

        const int MAX_DIGITS = 6;

        Stack<string> digits = new Stack<string>(MAX_DIGITS);

        public enum PinActions
        {
            IncorrectAnimation,
            Reset
        }

        protected readonly CompositeDisposable SubscriptionDisposables = new CompositeDisposable();

        public PinPadDigits()
        {
            InitializeComponent();
            SetupReactiveBehavior();
        }

        void SetupReactiveBehavior()
        {
            Observable
                .Merge(
                    Observable
                        .FromEventPattern(
                            x => button1.Clicked += x,
                            x => button1.Clicked -= x),
                    Observable
                        .FromEventPattern(
                            x => button2.Clicked += x,
                            x => button2.Clicked -= x),
                    Observable
                        .FromEventPattern(
                            x => button3.Clicked += x,
                            x => button3.Clicked -= x),
                    Observable
                        .FromEventPattern(
                            x => button4.Clicked += x,
                            x => button4.Clicked -= x),
                    Observable
                        .FromEventPattern(
                            x => button5.Clicked += x,
                            x => button5.Clicked -= x),
                    Observable
                        .FromEventPattern(
                            x => button6.Clicked += x,
                            x => button6.Clicked -= x),
                    Observable
                        .FromEventPattern(
                            x => button7.Clicked += x,
                            x => button7.Clicked -= x),
                    Observable
                        .FromEventPattern(
                            x => button8.Clicked += x,
                            x => button8.Clicked -= x),
                    Observable
                        .FromEventPattern(
                            x => button9.Clicked += x,
                            x => button9.Clicked -= x),
                    Observable
                        .FromEventPattern(
                            x => button0.Clicked += x,
                            x => button0.Clicked -= x))
                .Select(digit => ((Button)digit.Sender).Text)
                .Do(text => DigitAdded(text))
                .Subscribe()
                .DisposeWith(SubscriptionDisposables);

            Observable
                .FromEventPattern(
                    x => buttonImg.Clicked += x,
                    x => buttonImg.Clicked -= x)
                .Do(_ => DigitRemoved())
                .Subscribe()
                .DisposeWith(SubscriptionDisposables);
        }

        void DigitAdded(string index)
        {
            if (digits.Count < MAX_DIGITS)
            {
                digits.Push(index);
                Debug.WriteLine($"[DigitAdded] Added digit: {index}");
                int idx = digits.Count;
                ColorDigitTo(idx, DigitOnColor);
            }
            // Validate PIN
            if (digits.Count == MAX_DIGITS)
            {
                Stack stackReverse = new Stack(digits.ToArray());
                string pin = string.Join(string.Empty, stackReverse.ToArray());
                Debug.WriteLine($"[PinButtonClicked][Validate PIN] pin: {pin}");
            }
        }
       
        void DigitRemoved()
        {
            int idx = digits.Count;
            Debug.WriteLine($"[DigitRemoved] Remove digit: {idx}");

            string resultPop;
            digits.TryPop(out resultPop);

            if (resultPop?.Length > 0)
                ColorDigitTo(idx, DigitOffColor);
        }

        void ColorDigitTo(int index, Color color)
        {
            switch (index)
            {
                case 1:
                    Pin1.Fill = color;
                    break;
                case 2:
                    Pin2.Fill = color;
                    break;
                case 3:
                    Pin3.Fill = color;
                    break;
                case 4:
                    Pin4.Fill = color;
                    break;
                case 5:
                    Pin5.Fill = color;
                    break;
                case 6:
                    Pin6.Fill = color;
                    break;
            }
        }
    }
}