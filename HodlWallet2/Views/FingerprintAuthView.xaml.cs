using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet2.Locale;
using HodlWallet2.Utils;

namespace HodlWallet2.Views
{
    public partial class FingerprintAuthView : ContentPage
    {
        public FingerprintAuthView()
        {
            InitializeComponent();
            SetLabels();
            SetSwitch();
        }

        private void SetLabels()
        {
            Title = Device.RuntimePlatform == Device.iOS ?
                    LocaleResources.SecurityCenter_fingerprintHeaderIOS : LocaleResources.SecurityCenter_fingerprintHeaderAndroid;

            Header.Text = LocaleResources.FingerprintAuth_header;

            SwitchLabel.Text = Device.RuntimePlatform == Device.iOS ? 
                               LocaleResources.FingerprintAuth_switchLabelIOS : SwitchLabel.Text = LocaleResources.FingerprintAuth_switchAndroid;

            SpendLabel.Text = LocaleResources.FingerprintAuth_spendingLimit;

            FormattedString limitString = new FormattedString();

            limitString.Spans.Add(new Span { Text = LocaleResources.FingerprintAuth_subheader, 
                                             ForegroundColor = (Color)App.Current.Resources["White"] });

            Span textButton = new Span { Text = Device.RuntimePlatform == Device.iOS ? 
                                         LocaleResources.FingerprintAuth_limitButtonIOS : LocaleResources.FingerprintAuth_limitButtonAndroid,
                                         ForegroundColor = (Color)App.Current.Resources["SyncGradientStart"] };

            // TODO: Add Span Tap Event
            // textButton.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(async () => await Navigation.PushModalAsync(new PinModalView(...))) });

            limitString.Spans.Add(textButton);

            LimitLabel.FormattedText = limitString;
        }

        private void SetSwitch()
        {
            if (SecureStorageProvider.HasFingerprintStatus())
            {
                if (SecureStorageProvider.GetFingerprintStatus() == "1")
                {
                    FingerprintSwitch.IsToggled = true;
                }
            }

            FingerprintSwitch.Toggled += (sender, e) =>
            {
                if (SecureStorageProvider.HasFingerprintStatus())
                {
                    if (SecureStorageProvider.GetFingerprintStatus() == "1")
                    {
                        SecureStorageProvider.SetFingerprintStatus("0");
                    }
                    else
                    {
                        SecureStorageProvider.SetFingerprintStatus("1");
                    }
                }
            };
        }
    }
}
