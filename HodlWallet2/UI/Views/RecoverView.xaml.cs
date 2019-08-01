using System;

using HodlWallet2.UI.Locale;

using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class RecoverView : ContentPage
    {
        public RecoverView(bool closeable = false)
        {
            InitializeComponent();

            SetLabels();

            if (closeable) return;

            ToolbarItems.Clear();
        }

        void SetLabels()
        {
            Title = LocaleResources.Recover_title;
            Header.Text = LocaleResources.Recover_header;
            Next.Text = LocaleResources.Recover_next;
        }

        void Next_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RecoverWalletEntryView());
        }

        void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
