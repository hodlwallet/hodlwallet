using System.ComponentModel;

using Xamarin.Forms;

using HodlWallet2.Core.ViewModels;
using System;

namespace HodlWallet2.UI.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class RootView : TabbedPage
    {
        public enum Tabs { Send, Receive, Home, Settings };

        public RootView()
        {
            InitializeComponent();

            CurrentPage = Children[(int)Tabs.Home];

            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            // Views (this is unused, but could be an example
            //MessagingCenter.Subscribe<SendView, Tabs>(this, "ChangeCurrentPageTo", ViewModelChangeCurrentPageTo);

            // View Models
            MessagingCenter.Subscribe<SendViewModel, Tabs>(this, "ChangeCurrentPageTo", ViewModelChangeCurrentPageTo);
            MessagingCenter.Subscribe<ReceiveViewModel, Tabs>(this, "ChangeCurrentPageTo", ViewModelChangeCurrentPageTo);
            MessagingCenter.Subscribe<SettingsViewModel, Tabs>(this, "ChangeCurrentPageTo", ViewModelChangeCurrentPageTo);
            MessagingCenter.Subscribe<HomeViewModel, Tabs>(this, "ChangeCurrentPageTo", ViewModelChangeCurrentPageTo);
            // Add more view models, as needed though

            // Add yours here.
        }

        void ViewModelChangeCurrentPageTo(BaseViewModel _, Tabs tab)
        {
            ChangeTabTo(tab);
        }

        void ViewModelChangeCurrentPageTo(ContentPage _, Tabs tab)
        {
            ChangeTabTo(tab);
        }

        void ChangeTabTo(Tabs tab)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                CurrentPage = Children[(int)tab];
            });
        }
    }
}
