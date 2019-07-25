using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class SetPinView : MvxContentPage<SetPinViewModel>
    {
        public SetPinView()
        {
            InitializeComponent();
        }

        void Handle_Success(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
