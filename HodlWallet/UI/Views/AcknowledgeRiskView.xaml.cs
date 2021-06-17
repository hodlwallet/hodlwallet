using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AcknowledgeRiskView : ContentPage
    {
        bool term1, term2, term3, term4;

        public AcknowledgeRiskView()
        {
            InitializeComponent();
            term1 = term2 = term3 = term4 = false;
            AckButton.IsEnabled = false;
        }

        void WalletInfoButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BackupRecoveryWordView());
        }

        void OnAckTerm1Checked(object sender, CheckedChangedEventArgs e)
        {
            term1 = e.Value;
            AllTermsChecked();
        }

        void OnAckTerm2Checked(object sender, CheckedChangedEventArgs e)
        {
            term2 = e.Value;
            AllTermsChecked();
        }

        void OnAckTerm3Checked(object sender, CheckedChangedEventArgs e)
        {
            term3 = e.Value;
            AllTermsChecked();
        }

        void OnAckTerm4Checked(object sender, CheckedChangedEventArgs e)
        {
            term4 = e.Value;
            AllTermsChecked();
        }

        void AllTermsChecked()
        { 
            AckButton.IsEnabled = term1 && term2 && term3 && term4;
        }
    }
}