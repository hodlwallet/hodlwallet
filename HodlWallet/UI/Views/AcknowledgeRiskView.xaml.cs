using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AcknowledgeRiskView : ContentPage
    {
        bool acknowledgeRiskTerm1Checked, acknowledgeRiskTerm2Checked, acknowledgeRiskTerm3Checked, acknowledgeRiskTerm4Checked = false;

        public AcknowledgeRiskView()
        {
            InitializeComponent();
            AcknowledgeRiskTermsViewButton.IsEnabled = false;
        }

        void AcknowledgeRiskTermsViewButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BackupRecoveryWordView());
        }

        /// Toggles checkbox anytime a label is tapped.
        /// Some room to improvement
        private void OnAcknowledgeRiskTermLabel_Tapped(object sender, EventArgs e)
        {
            Label label = sender as Label;
            if (label == AcknowledgeRiskTerm1_Label)
            {
                AcknowledgeRiskTerm1_Check.IsChecked = !AcknowledgeRiskTerm1_Check.IsChecked;
            }
            else if (label == AcknowledgeRiskTerm2_Label)
            {
                AcknowledgeRiskTerm2_Check.IsChecked = !AcknowledgeRiskTerm2_Check.IsChecked;
            }
            else if (label == AcknowledgeRiskTerm3_Label)
            {
                AcknowledgeRiskTerm3_Check.IsChecked = !AcknowledgeRiskTerm3_Check.IsChecked;
            }
            else if (label == AcknowledgeRiskTerm4_Label)
            {
                AcknowledgeRiskTerm4_Check.IsChecked = !AcknowledgeRiskTerm4_Check.IsChecked;
            }
        }

        /// Toggles a boolean variable everytime a check is checked. 
        /// When all checks are checked the NEXT button gets enabled.
        /// Some room to improvement
        private void OnAcknowledgeRiskTerm_Checked(object sender, CheckedChangedEventArgs e)
        {
            CheckBox termCheck = sender as CheckBox;

            if (termCheck == AcknowledgeRiskTerm1_Check)
            {
                acknowledgeRiskTerm1Checked = e.Value;
            }
            else if (termCheck == AcknowledgeRiskTerm2_Check)
            {
                acknowledgeRiskTerm2Checked = e.Value;
            }
            else if (termCheck == AcknowledgeRiskTerm3_Check)
            {
                acknowledgeRiskTerm3Checked = e.Value;
            }
            else if (termCheck == AcknowledgeRiskTerm4_Check)
            {
                acknowledgeRiskTerm4Checked = e.Value;
            }

            AllAcknowledgeRiskTermsChecked();
        }

        private void AllAcknowledgeRiskTermsChecked()
        {
            AcknowledgeRiskTermsViewButton.IsEnabled = acknowledgeRiskTerm1Checked
                                            && acknowledgeRiskTerm2Checked
                                            && acknowledgeRiskTerm3Checked
                                            && acknowledgeRiskTerm4Checked;
        }
    }
}