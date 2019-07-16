using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet2.Core.ViewModels;
using System.ComponentModel;

namespace HodlWallet2.Views
{
    [MvxTabbedPagePresentation(TabbedPosition.Root, NoHistory = false, WrapInNavigationPage = true)]
    public partial class HomeView : MvxTabbedPage<HomeViewModel>
    {
        public HomeView()
        {
            InitializeComponent();

            CurrentPageChanged += HomeView_CurrentPageChanged;
        }

        private void HomeView_CurrentPageChanged(object sender, EventArgs e)
        {
            Title = CurrentPage.Title;
        }

        private bool _FirstTime = true;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_FirstTime)
            {
                ViewModel.ShowInitialViewModelsCommand.ExecuteAsync(null);

                _FirstTime = false;
            }
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
        }
    }
}
