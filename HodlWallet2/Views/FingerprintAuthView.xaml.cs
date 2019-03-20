using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Essentials;

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
            if (Preferences.ContainsKey("FingerprintStatus"))
            {
                if (Preferences.Get("FingerprintStatus", false))
                {
                    FingerprintSwitch.IsToggled = true;
                }
            }

            FingerprintSwitch.Toggled += (sender, e) =>
            {
                if (Preferences.ContainsKey("FingerprintStatus"))
                {
                    if (Preferences.Get("FingerprintStatus", false))
                    {
                        Preferences.Set("FingerprintStatus", false);
                    }
                    else
                    {
                        Preferences.Set("FingerprintStatus", true);
                    }
                }
            };
        }
    }
}
