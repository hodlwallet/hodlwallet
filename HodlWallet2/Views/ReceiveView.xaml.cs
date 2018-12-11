using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet2.ViewModels;

namespace HodlWallet2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecieveView : ContentPage
    {
        private ReceiveViewModel _ViewModel;

        public RecieveView(ReceiveViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            _QrCode.Source = viewModel.BarcodeImageSource;
            _QrCode.Aspect = Aspect.Fill;

            _QrCode.BindingContext = viewModel.BarcodeImage;

            //Content = new ContentView { Content = viewModel.BarcodeImage };
        }

        public ReceiveViewModel ViewModel { get => BindingContext as ReceiveViewModel; }

    }
}