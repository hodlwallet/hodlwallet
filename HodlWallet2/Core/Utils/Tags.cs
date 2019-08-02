﻿using Xamarin.Forms;

namespace HodlWallet2.Core.Utils
{
    public class Tags
    {
        public static readonly BindableProperty TagProperty = BindableProperty.Create(
            "Tag",
            typeof(string),
            typeof(Tags),
            null);

        public static string GetTag(BindableObject bindable)
        {
            return (string)bindable.GetValue(TagProperty);
        }

        public static void SetTag(BindableObject bindable, string value)
        {
            bindable.SetValue(TagProperty, value);
        }
    }
}