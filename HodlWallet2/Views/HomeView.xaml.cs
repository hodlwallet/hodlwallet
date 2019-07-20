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
using MvvmCross.Binding.BindingContext;
using MvvmCross.ViewModels;
using HodlWallet2.Core.Interactions;
using MvvmCross.Base;

namespace HodlWallet2.Views
{
    [MvxTabbedPagePresentation(TabbedPosition.Root, NoHistory = true, WrapInNavigationPage = true)]
    public partial class HomeView : MvxTabbedPage<HomeViewModel>
    {
        /// <summary>
        /// Add tabs as we add views to show on the tab view
        /// </summary>
        public enum Tabs
        {
            Send,
            Receive,
            Home,
            Settings
        }

        bool _FirstTime = true;

        IMvxInteraction<SelectCurrentTab> _SelectTabInteraction;
        public IMvxInteraction<SelectCurrentTab> SelectTabInteraction
        {
            get => _SelectTabInteraction;
            set
            {
                if (_SelectTabInteraction != null)
                    _SelectTabInteraction.Requested -= OnSelectTabInteractionRequested;

                _SelectTabInteraction = value;
                _SelectTabInteraction.Requested += OnSelectTabInteractionRequested;
            }
        }

        public HomeView()
        {
            InitializeComponent();

            CurrentPageChanged += HomeView_CurrentPageChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_FirstTime)
            {
                ViewModel.ShowInitialViewModelsCommand.Execute(null);

                ChangePageTo((Tabs)ViewModel.InitialTab);

                _FirstTime = false;
            }

            CreateInteractionBindings();
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
        }

        void ChangePageTo(Tabs tab)
        {
            CurrentPage = Children[(int)tab];
        }

        void HomeView_CurrentPageChanged(object sender, EventArgs e)
        {
            Title = CurrentPage.Title;
        }

        void CreateInteractionBindings()
        {
            var set = this.CreateBindingSet<HomeView, HomeViewModel>();

            set.Bind(this)
                .For(view => view.SelectTabInteraction)
                .To(viewModel => viewModel.SelectTabInteraction)
                .OneWay();

            set.Apply();
        }

        void OnSelectTabInteractionRequested(object sender, MvxValueEventArgs<SelectCurrentTab> e)
        {
            var tab = e.Value?.Tab;

            if (tab == null)
            {
                tab = (int)Tabs.Home;
            }

            ChangePageTo((Tabs)tab);
        }
    }
}
