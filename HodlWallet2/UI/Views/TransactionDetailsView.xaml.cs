using System;

using Xamarin.Forms;

using HodlWallet2.Core.Models;
using HodlWallet2.Core.ViewModels;

namespace HodlWallet2.UI.Views
{
    public partial class TransactionDetailsView : ContentPage
    {
        public TransactionDetailsView(TransactionModel txModel)
        {
            InitializeComponent();

            var vm = (TransactionDetailsViewModel)BindingContext;

            vm.TransactionModel = txModel;
        }

        void Close_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
