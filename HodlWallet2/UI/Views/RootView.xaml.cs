using System.ComponentModel;

using Xamarin.Forms;

using HodlWallet2.Core.ViewModels;

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
            SubscribeToMessages();

            ChangeTabTo(Tabs.Home);
        }

        void SubscribeToMessages()
        {
            // Views (this is unused, but could be an example)
            MessagingCenter.Subscribe<SendView, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);

            // View Models
            MessagingCenter.Subscribe<SendViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<ReceiveViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<SettingsViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            MessagingCenter.Subscribe<HomeViewModel, Tabs>(this, "ChangeCurrentPageTo", ChangeCurrentPageTo);
            // Add more view models, as needed though

            // Add yours here.
        }

        void ChangeCurrentPageTo(BaseViewModel _, Tabs tab)
        {
            ChangeTabTo(tab);
        }

        void ChangeCurrentPageTo(ContentPage _, Tabs tab)
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
