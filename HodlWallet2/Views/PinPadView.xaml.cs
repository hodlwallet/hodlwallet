using System;

using Xamarin.Forms;

using HodlWallet2.Utils;
using HodlWallet2.ViewModels;


namespace HodlWallet2.Views
{
    public partial class PinPadView : ContentPage
    {

        public PinPadViewModel ViewModel { get { return BindingContext as PinPadViewModel; } }

        public PinPadView(PinPadViewModel viewModel)
        {
            InitializeComponent();
            viewModel._Navigation = Navigation;
            BindingContext = viewModel;
        }

    }
}
