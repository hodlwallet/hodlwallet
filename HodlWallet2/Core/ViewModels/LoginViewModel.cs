﻿//
// LoginViewModel.cs
//
// Copyright (c) 2019 HODL Wallet
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

using HodlWallet2.Core.Services;
using HodlWallet2.UI.Locale;

using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        List<int> _Pin = new List<int>();
        object _Lock = new object();

        public ICommand DigitCommand { get; }
        public ICommand BackspaceCommand { get; }

        string _Header;
        public string Header
        {
            get => _Header;
            set => SetProperty(ref _Header, value);
        }

        public LoginViewModel()
        {
            Header = LocaleResources.Pin_enter;

            DigitCommand = new Command<string>((s) => _ = AddDigit(int.Parse(s)));
            BackspaceCommand = new Command(RemoveDigit);
        }

        async Task AddDigit(int digit)
        {
            Debug.WriteLine($"[AddDigit] Adding: {digit}");

            // Digit has already being inputed
            if (_Pin.Count >= 6) return;

            lock (_Lock)
            {
                _Pin.Add(digit);
            }

            MessagingCenter.Send(this, "DigitAdded", _Pin.Count);

            // Digit is not complete, input more
            if (_Pin.Count != 6) return;

            await Task.Delay(350);

            // _Pin.Count == 6 now...
            // We're done inputting our PIN

            string input = string.Join(string.Empty, _Pin.ToArray());

            // Check if it's the pin
            if (SecureStorageService.GetPin() == input)
            {
                Debug.WriteLine("[AddDigit] Logged in!");

                Header = string.Empty;
                IsLoading = true;

                // DONE! We navigate to the root view
                await Task.Delay(65);
                MessagingCenter.Send(this, "NavigateToRootView");
                
                return;
            }
            else
            {
                await Task.Delay(65);
                MessagingCenter.Send(this, "ResetPin");
            }

            Debug.WriteLine($"[AddDigit] Incorrect PIN: {input}");

            // Sadly it's not the pin! We clear and launch an animation
            _Pin.Clear();

            MessagingCenter.Send(this, "IncorrectPinAnimation");
        }

        void RemoveDigit()
        {
            Debug.WriteLine("[RemoveDigit]");

            if (_Pin.Count <= 0) return;

            lock (_Lock)
            {
                _Pin.RemoveAt(_Pin.Count - 1);

                MessagingCenter.Send(this, "DigitRemoved", _Pin.Count + 1);
            }
        }
    }
}
