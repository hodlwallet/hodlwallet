using System;

using Xamarin.Forms;

namespace HodlWallet2.Core.Utils
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

