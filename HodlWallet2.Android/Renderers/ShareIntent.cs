using System;
using HodlWallet2.Core.Utils;
using HodlWallet2.Core.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content.PM;
using Android.OS;
using Android;
using Android.Content;
using Android.App;
using Android.Graphics;
using Android.Provider;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.Design.Widget;
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
            MainActivity.Instance.StartActivity(intentChooser);
        }

        public void QRTextShareIntent(string address)
        {
            var intent = new Intent(Intent.ActionSend);
            intent.SetType(Constants.TEXT_PLAIN_INTENT_TYPE);
            intent.PutExtra(Intent.ExtraText, address);
            var intentChooser = Intent.CreateChooser(intent, Constants.SHARE_TEXT_INTENT_TITLE);
            MainActivity.Instance.StartActivity(intentChooser);
        }

        /* TODO: Make function public for IShareIntent.
         * Permission to access external storage is
         * required to generate a QR code and send it
         * via share intent. This is not implemented.
         */

        /*
        public void QRTextShareIntent(string address)
        {
            var intent = new Intent(Intent.ActionSend);
            intent.PutExtra(Intent.ExtraText, address);
            intent.SetType(Constants.IMAGE_PNG_INTENT_TYPE);

            var barcodeWrite = new ZXing.Mobile.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 300
                }
            };

            Bitmap bitmap = barcodeWrite.Write(address);

            // TODO: Use isPermissible
            bool isPermissible = GetStoragePermission();

            var path = Android.OS.Environment.GetExternalStoragePublicDirectory
            (
                Android.OS.Environment.DirectoryPictures + Java.IO.File.Separator + address + Constants.IMAGE_PNG_ADDRESS_NAME
            );

            using (var os = new System.IO.FileStream(path.AbsolutePath, System.IO.FileMode.Create))
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, os);
            }

            var fileUri = Android.Support.V4.Content.FileProvider.GetUriForFile(MainActivity.Instance, MainActivity.Instance.PackageName + ".fileprovider", path);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.PutExtra(Intent.ExtraStream, fileUri);
            MainActivity.Instance.StartActivity(Intent.CreateChooser(intent, Constants.SHARE_TEXT_INTENT_TITLE));
        }

        private bool GetStoragePermission()
        {
            const string permission = Manifest.Permission.WriteExternalStorage;
            if (MainActivity.Instance.CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                return true;
            }
            if (MainActivity.Instance.ShouldShowRequestPermissionRationale(permission))
            {
                //Snackbar.Make(, "Storage access is required to generate a QR Code.", Snackbar.LengthIndefinite)
                  // .SetAction("OK", v => ActivityCompat.RequestPermissions(MainActivity.Instance, new string[] { Manifest.Permission.WriteExternalStorage }, 1))
                  //  .Show();
            }

            return false;
        }
        */
    }
}