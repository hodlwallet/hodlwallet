using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ZXing.Common;
using Xamarin.Essentials;

using HodlWallet2.ViewModels;
using HodlWallet2.Locale;

namespace HodlWallet2.Views
{
    public partial class ReceiveView : ContentPage
    {
        private ReceiveViewModel _ViewModel;
        public EncodingOptions BarcodeOptions => new EncodingOptions() { Height = 300, Width = 300, PureBarcode = true };

        public ReceiveView(ReceiveViewModel receiveViewModel)
        {
            InitializeComponent();

            _ViewModel = receiveViewModel;

            BindingContext = _ViewModel;

            SetLabels();
        }

        public ReceiveViewModel ViewModel { get => BindingContext as ReceiveViewModel; }

        void SetLabels()
        {
            Title = LocaleResources.Receive_title;
            Share.Text = LocaleResources.Receive_share;
            RequestAmount.Text = LocaleResources.Receive_requestAmount;
        }

        public async void OnCloseTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        public async void OnFaqTapped(object sender, EventArgs e)
        {
            // TODO:
        }

        public async void OnAddressTapped(object sender, EventArgs e)
        {
            await ViewModel.ToClipboard();
        }
    }
}