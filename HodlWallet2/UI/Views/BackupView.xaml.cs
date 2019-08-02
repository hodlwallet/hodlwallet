using System;

using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class BackupView : ContentPage
    {
        public BackupView(string action = null)
        {
            InitializeComponent();
            EnableToolBarItems(action);
        }

        void EnableToolBarItems(string action = null)
        {
            switch (action)
            {
                case "close":
                    ToolbarItems.RemoveAt(1); // Remove Skip
                    break;
                case "skip":
                    ToolbarItems.RemoveAt(0); // Remove close
                    break;
                default:
                    ToolbarItems.Clear();
                    break;
            }
        }

        void BackupWordsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BackupRecoveryWordView());
        }

        void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void SkipToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RootView());
        }
    }
}
