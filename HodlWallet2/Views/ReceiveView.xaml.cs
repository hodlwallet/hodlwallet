using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet2.ViewModels;
using ZXing.Common;

namespace HodlWallet2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecieveView : ContentPage
    {
        private ReceiveViewModel _ViewModel;
        public EncodingOptions BarcodeOptions => new EncodingOptions() { Height = 300, Width = 300, PureBarcode = true };

        public RecieveView()
        {
            InitializeComponent();

            _ViewModel = new ReceiveViewModel();

            BindingContext = _ViewModel;
        }

        public ReceiveViewModel ViewModel { get => BindingContext as ReceiveViewModel; }

    }
}