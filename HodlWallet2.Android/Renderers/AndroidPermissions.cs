using HodlWallet2.Core.Interfaces;
using HodlWallet2.Droid.Renderers;
using Xamarin.Forms;

[assembly: Dependency (typeof (AndroidPermissions))]
namespace HodlWallet2.Droid.Renderers
{
    public class AndroidPermissions : IPermissions
    {
        public bool HasCameraPermission()
        {
            var needsPermissionRequest = ZXing.Net.Mobile.Android.PermissionsHandler.NeedsPermissionRequest(MainActivity.Instance);

            if (needsPermissionRequest)
                ZXing.Net.Mobile.Android.PermissionsHandler.RequestPermissionsAsync(MainActivity.Instance);

            if (!needsPermissionRequest)
                return true;

            return false;
        }
    }
}
