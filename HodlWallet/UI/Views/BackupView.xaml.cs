//
// BackupView.xaml.cs
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
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HodlWallet.UI.Views
{
    public partial class BackupView : ContentPage
    {
        AppShell appShell;

        public BackupView(string action = null)
        {
            InitializeComponent();
            EnableToolBarItems(action);

            Task.Run(() =>
            {
                appShell = new AppShell();
            });
        }


        void EnableToolBarItems(string action = null)
        {
            switch (action)
            {
                case "close":
                    ToolbarItems.RemoveAt(1); // Remove Skip
                    break;
                case "skip":
                    ToolbarItems.RemoveAt(0); // Remove close
                    break;
                default:
                    ToolbarItems.Clear();
                    break;
            }
        }

        void BackupWordsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BackupRecoveryWordView());
        }

        void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void SkipToolbarItem_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = appShell ?? new AppShell();
        }
    }
}
