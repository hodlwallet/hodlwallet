using System;

using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class BackupView : ContentPage
    {
        public BackupView(bool closable = false)
        {
            InitializeComponent();

            if (closable) return;

            ToolbarItems.Clear();
        }

        void BackupWordsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BackupRecoveryWordView());
        }

        void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
