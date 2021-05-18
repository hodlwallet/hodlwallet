using HodlWallet.UI.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewWalletInfoView : ContentPage
    {
        public NewWalletInfoView()
        {
            InitializeComponent();
            SetLabels();
        }

        void SetLabels()
        {
            //Title = LocaleResources.Backup_title;
            Header.Text = LocaleResources.NewWallet_header;
            Subheader.Text = LocaleResources.NewWallet_subheader;
            Button.Text = LocaleResources.NewWallet_button;
        }

        void WalletInfoButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AcknowledgeRiskView());
        }
    }
}