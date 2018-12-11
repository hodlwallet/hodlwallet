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

            _ViewModel = viewModel;
            BindingContext = viewModel;

            Content = new ContentView() { Content = viewModel.GetBarcodeImage() };
        }

        public ReceiveViewModel ViewModel { get => BindingContext as ReceiveViewModel; }

    }
}