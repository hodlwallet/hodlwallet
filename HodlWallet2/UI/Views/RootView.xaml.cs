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
            MessagingCenter.Subscribe<SendViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<ReceiveViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<SettingsViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<HomeViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);

            // Add yours here.
        }

        void ChangeCurrentPageTo(BaseViewModel _, Tabs tab)
        {
            CurrentPage = Children[(int)tab];
        }
    }
}