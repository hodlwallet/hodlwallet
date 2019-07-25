using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    [MvxTabbedPagePresentation(TabbedPosition.Tab, WrapInNavigationPage = false, Title = "")]
    public partial class HomeView : MvxContentPage<HomeViewModel>
    {
        public HomeView()
        {
            IconImageSource = "home_tab.png";

            InitializeComponent();
        }
    }
}
