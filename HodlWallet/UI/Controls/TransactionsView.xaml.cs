using System;

using Xamarin.Forms;

namespace HodlWallet.UI.Controls
{
    public partial class TransactionsView : StackLayout
    {
        public TransactionsView()
        {
            InitializeComponent();
        }

        void EmptyGetAddressButton_Clicked(object sender, EventArgs e)
        {
            (Shell.Current as AppShell).ChangeTabsTo("bitcoin");
        }
    }
}