using System;
using System.Collections.Generic;
using System.Linq;
using HodlWallet2.Core.Services;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace HodlWallet2.UI.Renderers
{
    public partial class CustomNavigationPage : MvxNavigationPage
    {
        public CustomNavigationPage() : base()
        {
            InitializeComponent();
        }

        public CustomNavigationPage(Page root) : base(root) 
        {
            InitializeComponent();
        }

        public bool IgnoreLayoutChange { get; set; } = false;

        protected override void OnSizeAllocated(double width, double height) 
        {
            if (!IgnoreLayoutChange)
                base.OnSizeAllocated(width, height);
        }

        private void HelpToolbarItem_Clicked(object sender, EventArgs e)
        {
            Page view = Navigation.NavigationStack.LastOrDefault<Page>();

            if (view != null)
            {
                string articleName = GetArticleName(view.GetType().ToString());

                WalletService.Instance.Logger.Information("TODO: Open help article for {articleName}", articleName);
            }
        }

        private string GetArticleName(string viewName)
        {
            string result = viewName.Split('.').LastOrDefault();

            if (result != null)
            {
                result = result.Replace("View", "");
            }

            return result;
        }
    }
}
