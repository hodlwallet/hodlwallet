using System;

using Xamarin.Forms;

namespace HodlWallet2.Utils
{
    public class Tags 
    {
        public static readonly BindableProperty TagProperty = BindableProperty.CreateAttached("NextEntryTag", typeof(string), typeof(Tags), null);

        public static string GetTag(BindableObject view)
        {
            return (string)view.GetValue(TagProperty);
        }

        public static void SetNextEntryTag(BindableObject view, string value)
        {
            view.SetValue(TagProperty, value);
        }
    }
}

