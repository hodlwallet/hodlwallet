using Xamarin.Forms;

using ZXing.Net.Mobile.Forms;

namespace HodlWallet2.ViewModels
{
    public class ReceiveViewModel
    {
        public string Address { get; }

        public Image BarcodeImage
        {
            get
            {
                var barcodeImage = new ZXingBarcodeImageView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    AutomationId = "zxingBarcodeImageView",
                    Scale = .75
                };

                barcodeImage.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;

                barcodeImage.BarcodeOptions.Width = 300;
                barcodeImage.BarcodeOptions.Height = 300;

                barcodeImage.BarcodeOptions.Margin = 10;

                barcodeImage.BarcodeValue = Address;

                return barcodeImage;
            }
        }

        public ImageSource BarcodeImageSource
        {
            get
            {
                return BarcodeImage.Source;
            }
        }

        public ReceiveViewModel(string address)
        {
            Address = address;
        }
    }
}
