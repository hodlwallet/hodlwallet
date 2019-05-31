using System;
using System.Collections.Generic;

using HodlWallet2.Core.ViewModels;
using MvvmCross.Forms.Views;
using MvvmCross.Forms.Presenters.Attributes;

using Xamarin.Forms;

namespace HodlWallet2.Views
{
    public partial class TransactionCollectionView : MvxContentView<TransactionCollectionViewModel>
    {
        public TransactionCollectionView()
        {
            InitializeComponent();
        }
    }
}
