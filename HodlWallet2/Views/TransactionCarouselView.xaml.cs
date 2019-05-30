using System;
using System.Collections;

using HodlWallet2.Core.ViewModels;
using MvvmCross.Forms.Views;
using MvvmCross.Forms.Presenters.Attributes;

using Xamarin.Forms;

namespace HodlWallet2.Views
{
    [MvxModalPresentation]
    public partial class TransactionCarouselView : MvxContentView<TransactionCarouselViewModel>
    {
        public TransactionCarouselView()
        {
            InitializeComponent();
        }
    }
}