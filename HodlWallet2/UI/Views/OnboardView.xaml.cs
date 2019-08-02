using System;
using System.Diagnostics;

using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class OnboardView : ContentPage
    {
        public OnboardView()
        {
            InitializeComponent();
        }

        void CreateButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("[CreateButton_Clicked]");

            Navigation.PushAsync(new PinPadView());
        }

        void RecoverButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("[RecoverButton_Clicked]");

            Navigation.PushAsync(new RecoverView());
        }
    }
}
