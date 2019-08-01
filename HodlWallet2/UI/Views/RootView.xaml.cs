using System;
using System.ComponentModel;
using System.Linq;

using Xamarin.Forms;

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
        }
    }
}