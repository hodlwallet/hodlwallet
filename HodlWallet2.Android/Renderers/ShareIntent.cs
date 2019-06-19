using System;
using HodlWallet2.Core.Utils;
using HodlWallet2.Core.Interfaces;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Android.Content;
using Android.Graphics;

using HodlWallet2.Droid.Renderers;

[assembly: Xamarin.Forms.Dependency (typeof(ShareIntent))]
namespace HodlWallet2.Droid.Renderers
{
    public class ShareIntent : IShareIntent
    {
        public void TextShareIntent(string text)
        {
            var intent = new Intent(Intent.ActionSend);
            intent.SetType(Constants.TEXT_PLAIN_INTENT_TYPE);
            intent.PutExtra(Intent.ExtraText, text);
            var intentChooser = Intent.CreateChooser(intent, Constants.SHARE_TEXT_INTENT_TITLE);
			Forms.Context.StartActivity(intentChooser);
        }
    }
}
