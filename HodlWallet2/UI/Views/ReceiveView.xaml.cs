using System;

using Xamarin.Essentials;
using Xamarin.Forms;

using HodlWallet2.Core.ViewModels;
using HodlWallet2.Core.Utils;
using HodlWallet2.UI.Extensions;

namespace HodlWallet2.UI.Views
{
    public partial class ReceiveView : ContentPage
    {
        ReceiveViewModel _ViewModel;

        public ReceiveView()
        {
            InitializeComponent();

            _ViewModel = (ReceiveViewModel)BindingContext;
        }

        void Address_Tapped(object sender, EventArgs e)
        {
            Clipboard.SetTextAsync(_ViewModel.Address);

            _ = this.DisplayToast(Constants.RECEIVE_ADDRESS_COPIED_TO_CLIPBOARD_TITLE);

            //DisplayAlert(
            //    Constants.RECEIVE_ADDRESS_COPIED_TO_CLIPBOARD_TITLE,
            //    "",
            //    Constants.RECEIVE_ADDRESS_COPIED_TO_CLIPBOARD_BUTTON
            //);
        }
    }
}
