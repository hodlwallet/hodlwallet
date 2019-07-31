using Xamarin.Forms;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.iOS.Renderers;

[assembly: Dependency(typeof (iOSPermissions))]
namespace HodlWallet2.iOS.Renderers
{
    public class iOSPermissions : IPermissions
    {
        public bool HasCameraPermission()
        {
            // FIXME "this cannot be correct...
            // I thought it was doing something" - Igor.
            return true;
        }
    }
}
