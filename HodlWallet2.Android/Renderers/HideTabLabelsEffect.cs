using System.Linq;

using Android.Support.Design.BottomNavigation;
using Android.Support.Design.Widget;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

using HodlWallet2.Droid.Renderers;

[assembly: ExportEffect(typeof(HideTabLabelsEffect), nameof(HideTabLabelsEffect))]
namespace HodlWallet2.Droid.Renderers
{
    public class HideTabLabelsEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var renderer = (Control ?? Container) as TabbedPageRenderer;

            var children = renderer?.ViewGroup?.RetrieveAllChildViews();
            if (children?.FirstOrDefault(x => x is BottomNavigationView) is BottomNavigationView bottomNav)
            {
                bottomNav.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityUnlabeled;
            }
        }

        protected override void OnDetached()
        {
        }
    }
}
