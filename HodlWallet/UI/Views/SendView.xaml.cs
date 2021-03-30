//
// SendView.xaml.cs
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
using System.Threading.Tasks;

using Xamarin.Forms;

using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

using HodlWallet.Core.Utils;
using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Extensions;
using HodlWallet.UI.Locale;

namespace HodlWallet.UI.Views
{
    public partial class SendView : ContentPage
    {
        public SendView()
        {
            InitializeComponent();

            SubscribeToMessages();

            SetLabels();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<SendViewModel>(this, "OpenBarcodeScanner", async (vm) => await OpenBarcodeScanner(vm));
            MessagingCenter.Subscribe<SendViewModel, string[]>(this, "DisplayProcessAlertError", DisplayProcessAlertError);
            MessagingCenter.Subscribe<SendViewModel, ValueTuple<decimal, decimal>>(this, "AskToBroadcastTransaction", AskToBroadcastTransaction);
        }

        async void AskToBroadcastTransaction(SendViewModel vm, (decimal, decimal) values)
        {
            decimal totalOut = values.Item1;
            decimal fees = values.Item2;
            var total = totalOut + fees;

            string title = LocaleResources.Send_transaction;

            string message = string.Format(LocaleResources.Send_transactionMessage, total, totalOut, fees, SendAddress.Text);
            string okButton = LocaleResources.Send_transactionOk;
            string cancelButton = LocaleResources.Send_transactionCancel;

            var res = await this.DisplayPrompt(title, message, okButton, cancelButton);

            if (!res) return;

            MessagingCenter.Send(this, "BroadcastTransaction");
        }

        void SetLabels()
        {
            ToLabel.Text = LocaleResources.Send_to;
            ScanLabel.Text = LocaleResources.Send_scan;
            PasteLabel.Text = LocaleResources.Send_paste;
            AmountLabel.Text = LocaleResources.Send_amount;
            ISOLabel.Text = "USD($)"; // Localize
        }

        async Task OpenBarcodeScanner(SendViewModel _)
        {
            // Android
            if (Device.RuntimePlatform == Device.Android)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    MobileBarcodeScanner scanner = new();
                    ZXing.Result resultAndroid = await scanner.Scan();

                    MessagingCenter.Send(this, "BarcodeScannerResult", resultAndroid.Text);
                });
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // TODO These definitions should be moved to another place...
                //var customOverlay = new StackLayout
                //{
                //    HorizontalOptions = LayoutOptions.FillAndExpand,
                //    VerticalOptions = LayoutOptions.FillAndExpand
                //};
                //var torch = new Button
                //{
                //    Text = "Toggle Torch"
                //};
                //torch.Clicked += delegate {
                //    _ScanPage.ToggleTorch();
                //};
                //customOverlay.Children.Add(torch);

                ZXingScannerPage scanPage = new(
                    //customOverlay: customOverlay,
                    new MobileBarcodeScanningOptions
                    {
                        AutoRotate = true,
                        UseNativeScanning = true,
                        DelayBetweenAnalyzingFrames = 5,
                        DelayBetweenContinuousScans = 5
                    }
                );

                scanPage.Title = "Scan Bitcoin Address";

                scanPage.OnScanResult += (ZXing.Result resultIOS) =>
                {
                    scanPage.IsScanning = false;

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        MessagingCenter.Send(this, "BarcodeScannerResult", resultIOS.Text);

                        await Navigation.PopAsync();
                    });
                };

                await Navigation.PushAsync(scanPage);
            }
            else
            {
                throw new ArgumentException($"Platform {Device.RuntimePlatform} not supported");
            }
        }

        void DisplayProcessAlertError(SendViewModel _, string[] messageAndTitle)
        {
            string title = messageAndTitle[0] ?? Constants.DISPLAY_ALERT_ERROR_TITLE;
            string message = messageAndTitle[1];

            Device.BeginInvokeOnMainThread(async () =>
            {
                await this.DisplayToast(message ?? string.Join("", messageAndTitle));
            });
        }
    }
}
