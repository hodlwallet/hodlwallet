using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HodlWallet2.Core.Utils;
using HodlWallet2.Core.ViewModels;
using HodlWallet2.UI.Locale;
using NBitcoin.Protocol;
using Xamarin.Forms;

using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace HodlWallet2.UI.Views
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
                MobileBarcodeScanner scanner = new MobileBarcodeScanner();
                ZXing.Result resultAndroid = await scanner.Scan();

                MessagingCenter.Send(this, "BarcodeScannerResult", resultAndroid.Text);
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

                ZXingScannerPage scanPage = new ZXingScannerPage(
                    //customOverlay: customOverlay,
                    new MobileBarcodeScanningOptions
                    {
                        AutoRotate = true,
                        UseNativeScanning = true,
                        DelayBetweenAnalyzingFrames = 5,
                        DelayBetweenContinuousScans = 5
                    }
                );

                scanPage.OnScanResult += (ZXing.Result resultIOS) =>
                {
                    scanPage.IsScanning = false;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopAsync();

                        MessagingCenter.Send(this, "BarcodeScannerResult", resultIOS.Text);
                    });
                };
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

            DisplayAlert(
                title,
                message,
                Constants.RECEIVE_ADDRESS_COPIED_TO_CLIPBOARD_BUTTON
            );
        }
    }
}
