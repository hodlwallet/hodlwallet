using System;

using Xamarin.Forms;

namespace HodlWallet2.Utils
{
    public class Tags 
    {
        public static readonly BindableProperty NextEntryTagProperty = BindableProperty.CreateAttached("NextEntryTag", typeof(string), typeof(Tags), null);

        public static string GetNextEntryTag(BindableObject view)
        {
            return (string)view.GetValue(NextEntryTagProperty);
        }

        public static void SetNextEntryTag(BindableObject view, string value)
        {
            view.SetValue(NextEntryTagProperty, value);
        }
    }
}

