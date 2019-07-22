using System.Collections.Generic;
using System.Linq;
using Android.Views;

namespace HodlWallet2.Droid.Renderers
{
    public static class ViewExtensions
    {
        public static List<View> RetrieveAllChildViews(this View view)
        {
            if (!(view is ViewGroup group))
            {
                return new List<View> { view };
            }

            var result = new List<View>();

            for (var i = 0; i < group.ChildCount; i++)
            {
                var child = group.GetChildAt(i);

                var childList = new List<View> { child };
                childList.AddRange(RetrieveAllChildViews(child));

                result.AddRange(childList);
            }

            return result.Distinct().ToList();
        }
    }
}
