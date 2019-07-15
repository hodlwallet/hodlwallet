using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace HodlWallet2.Views
{
    [MvxTabbedPagePresentation(TabbedPosition.Tab, WrapInNavigationPage = false, Title = "Send")]
    public partial class SendTabView : MvxContentPage<SendTabViewModel>
    {
        public SendTabView()
        {
            IconImageSource = "send_tab.png";

            InitializeComponent();
        }
    }
}
