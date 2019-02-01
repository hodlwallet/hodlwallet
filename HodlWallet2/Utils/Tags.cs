using System;

using Xamarin.Forms;

namespace HodlWallet2.Utils
{
    public class Tags : ContentPage
    {
        public Tags()
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

