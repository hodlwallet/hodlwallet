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

            //_ViewModel = viewModel;

            //_Address.Text = _ViewModel.Address;
            //_QrCode.Content = new ContentView() { Content = _ViewModel.GetBarcodeImage() };

            //Content = new ContentView() { Content = _ViewModel.GetBarcodeImage() };
        }

        public ReceiveViewModel ViewModel { get => BindingContext as ReceiveViewModel; }

    }
}