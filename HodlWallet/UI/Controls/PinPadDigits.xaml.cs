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
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet.Core.Interfaces;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PinPadDigits
    {
        IAuthenticationService AuthenticationService => DependencyService.Get<IAuthenticationService>();

        Color DigitOnColor => (Color)Application.Current.Resources["FgSuccess"];

        Color DigitOffColor => (Color)Application.Current.Resources["Fg5"];

        const int MAX_DIGITS = 6;

        Stack<string> digits = new Stack<string>(MAX_DIGITS);
        
        public PinPadDigits()
        {
            InitializeComponent();
        }

        async void DigitClicked(object sender, EventArgs e)
        {
            string index = (sender as Button).Text;
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
                // Check if it's the pin
                if (!AuthenticationService.Authenticate(pin))
                {
                    // Whenever the pin is incorrect, clear the stack to fill out again
                    Debug.WriteLine($"[DigitAdded] Incorrect PIN - DO Animation.");
                    await IncorrectPinAnimation();
                    digits.Clear();
                    ColorOffPad();
                }
            }
        }

        private void BackSpaceClicked(object sender, EventArgs e)
        {
            int idx = digits.Count;
            Debug.WriteLine($"[DigitRemoved] Remove digit: {idx}");

            string resultPop;
            digits.TryPop(out resultPop);

            if (resultPop?.Length > 0)
                ColorDigitTo(idx, DigitOffColor);
        }

        void ColorOffPad()
        {
            Pin1.Fill = DigitOffColor;
            Pin2.Fill = DigitOffColor;
            Pin3.Fill = DigitOffColor;
            Pin4.Fill = DigitOffColor;
            Pin5.Fill = DigitOffColor;
            Pin6.Fill = DigitOffColor;
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

        readonly uint incorrectPinAnimationTimeout = 50;
        async Task IncorrectPinAnimation()
        {
            Debug.WriteLine("[IncorrectPinAnimation]");

            // Shake ContentView Re-Enter PIN
            await InputGrid.TranslateTo(-15, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(15, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(-10, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(10, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(-5, 0, incorrectPinAnimationTimeout);
            await InputGrid.TranslateTo(5, 0, incorrectPinAnimationTimeout);

            InputGrid.TranslationX = 0;
        }
    }
}