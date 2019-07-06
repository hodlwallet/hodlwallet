using Xamarin.Forms;

using ZXing.Net.Mobile.Android;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Droid.Renderers;

[assembly: Dependency (typeof (AndroidPermissions))]
namespace HodlWallet2.Droid.Renderers
{
    public class AndroidPermissions : IPermissions
    {
        public bool HasCameraPermission()
        {
            var needsPermissionRequest = PermissionsHandler.NeedsPermissionRequest(MainActivity.Instance);

            if (needsPermissionRequest) PermissionsHandler.RequestPermissionsAsync(MainActivity.Instance);

            return !needsPermissionRequest;
        }
    }
}
