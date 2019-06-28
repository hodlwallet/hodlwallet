using HodlWallet2.Core.Interfaces;
using HodlWallet2.iOS.Renderers;
using Xamarin.Forms;

[assembly: Dependency (typeof (iOSPermissions))]
namespace HodlWallet2.iOS.Renderers
{
    public class iOSPermissions : IPermissions
    {
        public bool HasCameraPermission()
        {
            return true;
        }
    }
}
