using System;

using Xamarin.Forms;

using HodlWallet2.Utils;
using HodlWallet2.Core.ViewModels;
using MvvmCross.Forms.Views;

namespace HodlWallet2.Views
{
    public partial class PinPadView : MvxContentPage<PinPadViewModel>
    {
        public PinPadView()
        {
            InitializeComponent();
        }

    }
}
