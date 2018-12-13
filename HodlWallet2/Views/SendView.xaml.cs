using System;
using System.Collections.Generic;
using HodlWallet2.ViewModels;
using Xamarin.Forms;

namespace HodlWallet2.Views
{
    public partial class SendView : ContentPage
    {
        public SendViewModel ViewModel { get { return BindingContext as SendViewModel; } }

        public SendView()
        {
            InitializeComponent();
        }

        public SendView(SendViewModel svm)
        {
            InitializeComponent();
            BindingContext = svm;

        }

        public async void Scan()
        {
            var address = await ViewModel.Scan();
            sendAddress.Text = address;
        }

        public async void Send()
        {
            ViewModel.Send();
        }

    }
}
