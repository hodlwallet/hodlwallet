using System;
using HodlWallet2.Core.Utils;
using HodlWallet2.Core.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Android.Graphics;
using Android.Provider;

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

        public void QRTextShareIntent(string address)
		{
			var intent = new Intent(Intent.ActionSend);
			intent.PutExtra(Intent.ExtraText, address);
			intent.SetType(Constants.IMAGE_PNG_INTENT_TYPE);

			var barcodeWriter = new ZXing.Mobile.BarcodeWriter
			{
				Format = ZXing.BarcodeFormat.QR_CODE,
				Options = new ZXing.Common.EncodingOptions
				{
					Width = 300,
					Height = 300
				}
			};
			Bitmap bitmap = barcodeWriter.Write(address);

			// TODO: Get Permission for HodlWallet to use external storage

			var path = Android.OS.Environment.GetExternalStoragePublicDirectory
			(
				Android.OS.Environment.DirectoryPictures + Java.IO.File.Separator + Constants.IMAGE_PNG_ADDRESS_NAME
			);

            using (var os = new System.IO.FileStream(path.AbsolutePath, System.IO.FileMode.Create))
			{
				bitmap.Compress(Bitmap.CompressFormat.Png, 100, os);
			}

			var fileUri = Android.Support.V4.Content.FileProvider.GetUriForFile(global::Android.App.Application.Context, global::Android.App.Application.Context.PackageName + ".fileprovider", path);
			intent.AddFlags(ActivityFlags.GrantReadUriPermission);
			intent.PutExtra(Intent.ExtraStream, fileUri);
			Forms.Context.StartActivity(Intent.CreateChooser(intent, Constants.SHARE_TEXT_INTENT_TITLE));
		}
    }
}