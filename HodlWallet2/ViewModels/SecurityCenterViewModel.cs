using System;

using Xamarin.Forms;

namespace HodlWallet2.ViewModels
{
    public class SecurityCenterViewModel : ContentPage
    {
        public SecurityCenterViewModel()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

