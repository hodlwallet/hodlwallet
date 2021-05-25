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
    public partial class AcknowledgeRiskView : ContentPage
    {
        public AcknowledgeRiskView()
        {
            InitializeComponent();
            SetLabels();
        }

        void SetLabels()
        {
            Title = LocaleResources.NewWallet_Title;
            AckTerm1.Text = LocaleResources.AckRisk_Term1;
            AckTerm2.Text = LocaleResources.AckRisk_Term2;
            AckTerm3.Text = LocaleResources.AckRisk_Term3;
            AckTerm4.Text = LocaleResources.AckRisk_Term4;
            AckButton.Text = LocaleResources.AckRisk_Button;
        }

        void WalletInfoButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BackupRecoveryWordView());
        }
    }
}