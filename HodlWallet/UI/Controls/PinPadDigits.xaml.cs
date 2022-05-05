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
using System.Collections.Generic;
using System.Diagnostics;

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

        Stack<string> numbers = new Stack<string>(MAX_DIGITS);

        public PinPadDigits()
        {
            InitializeComponent();
        }

        public void PinButtonClicked(object sender, EventArgs e)
        {
            Button digit = sender as Button;
            string index = digit.Text;
            Debug.WriteLine($"[PinButtonClicked][Event] Digit: {index}");
            DigitAdded(index);
        }

        public void BackspaceClicked(object sender, EventArgs e)
        {
            Debug.WriteLine($"[BackspaceClicked][Event] Call remove...");
            DigitRemoved();
        }

        void DigitAdded(string index)
        {
            Debug.WriteLine($"[DigitAdded] Try to add: {index} - ableToAddDigit?: {numbers.Count < MAX_DIGITS} ");
            if (numbers.Count < MAX_DIGITS)
            {
                numbers.Push(index);
                Debug.WriteLine($"[DigitAdded] Added digit: {index}");
                int idx = numbers.Count;
                ColorDigitTo(idx, DigitOnColor);
            }
            // Validate PIN
            if (numbers.Count == MAX_DIGITS)
                Debug.WriteLine($"[PinButtonClicked][Validate PIN]");
        }

        void DigitRemoved()
        {
            int idx = numbers.Count;
            Debug.WriteLine($"[DigitRemoved] Remove digit: {idx}");

            string resultPop;
            numbers.TryPop(out resultPop);

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