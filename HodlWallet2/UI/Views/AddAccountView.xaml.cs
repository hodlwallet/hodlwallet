using System;

using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class AddAccountView : ContentPage
    {
        public AddAccountView(string action = null)
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
                default:
                    ToolbarItems.Clear();
                    break;
            }
        }
        void AddAccountButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BackupRecoveryWordView());
        }

        void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
