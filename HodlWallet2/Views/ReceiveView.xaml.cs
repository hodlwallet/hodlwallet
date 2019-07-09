using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ZXing.Common;
using Xamarin.Essentials;

using HodlWallet2.Locale;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

namespace HodlWallet2.Views
{
    [MvxModalPresentation]
    public partial class ReceiveView : MvxContentPage<ReceiveViewModel>
    {
       
        public ReceiveView()
        {
            InitializeComponent();
        }

        void SetLabels()
        {
            ReceiveTitle.Text = LocaleResources.Receive_title;
            Share.Text = LocaleResources.Receive_share;
            //RequestAmount.Text = LocaleResources.Receive_requestAmount;
        }

        public async void OnCloseTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}