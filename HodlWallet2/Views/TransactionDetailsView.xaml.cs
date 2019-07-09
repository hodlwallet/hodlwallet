using System;
using System.Collections.Generic;

using Xamarin.Forms;

using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;

using HodlWallet2.Core.ViewModels;

namespace HodlWallet2.Views
{
    [MvxModalPresentation]
    public partial class TransactionDetailsView : MvxContentPage<TransactionDetailsViewModel>
    {
        public TransactionDetailsView()
        {
            InitializeComponent();
        }
    }
}