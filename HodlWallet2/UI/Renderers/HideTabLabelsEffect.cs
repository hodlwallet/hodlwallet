using Xamarin.Forms;

namespace HodlWallet2.UI.Renderers
{
    public class HideTabLabelsEffect : RoutingEffect
    {
        public HideTabLabelsEffect()
            : base($"AppEffects.{nameof(HideTabLabelsEffect)}")
        {
        }
    }
}
