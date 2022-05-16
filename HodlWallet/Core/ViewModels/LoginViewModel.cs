//
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
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;

using ReactiveUI;
using System.Threading;

namespace HodlWallet.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        readonly CancellationTokenSource cts = new();
        public string Action { get; set; }
        
        bool loginFormVisible = false;
        public bool LoginFormVisible
        {
            get
            {
                return loginFormVisible;
            }

            set
            {
                loginFormVisible = SetProperty(ref loginFormVisible,value);

                AuthenticationService.ShowingLoginForm = loginFormVisible;
            }
        }

        internal static void SetLastLoginAs(string lastLogin)
        {
            Preferences.Set("lastLogin", lastLogin);
        }

        bool biometricsAvailable;
        public bool BiometricsAvailable
        {
            get
            {
                return AuthenticationService.BiometricsAvailable;
            }

            set
            {
                SetProperty(ref biometricsAvailable, value);
                AuthenticationService.BiometricsAvailable = biometricsAvailable;
            }
        }
        
        string lastLogin;
        public string LastLogin
        {
            get
            {
                if (lastLogin is null)
                    lastLogin = Preferences.Get("lastLogin", "pin");

                return lastLogin;
            }
            set
            {
                SetProperty(ref lastLogin, value);
                SetLastLoginAs(lastLogin);
            }
        }


        public LoginViewModel()
        {
            AuthenticationService
               .WhenAnyValue(x => x.IsAuthenticated)
               .Where(x => x is true)
               .Do(async _ => await AuthenticationProcess())
               .Subscribe(cts.Token);
        }

        async Task AuthenticationProcess()
        {
            if (Action == "update")
            {
                Debug.WriteLine("[AuthenticationProcess] Authenticated!");
                MessagingCenter.Send(this, "UpdatePin");
            }
            else
            {
                Debug.WriteLine("[AuthenticationProcess] Logged in!");

                IsLoading = true;

                // DONE! We navigate to the root view
                await Task.Delay(65);
                MessagingCenter.Send(this, "StartAppShell");
            }
        }        
    }
}
