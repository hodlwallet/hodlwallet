using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet2.ViewModels;
using ZXing.Common;

namespace HodlWallet2.Views
{
    public partial class RecieveView : ContentPage
    {
        private ReceiveViewModel _ViewModel;
        public EncodingOptions BarcodeOptions => new EncodingOptions() { Height = 300, Width = 300, PureBarcode = true };

        public RecieveView(ReceiveViewModel receiveViewModel)
        {
            InitializeComponent();

            _ViewModel = receiveViewModel;

            BindingContext = _ViewModel;
        }

        public ReceiveViewModel ViewModel { get => BindingContext as ReceiveViewModel; }

    }
}