using System;

using Xamarin.Forms;

namespace HodlWallet2.Utils
{
    public class RecoverEntry : ContentPage
    {
        public RecoverEntry()
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

