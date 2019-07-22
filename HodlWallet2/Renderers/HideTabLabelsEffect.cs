using Xamarin.Forms;

namespace HodlWallet2.Renderers
{
    public class HideTabLabelsEffect : RoutingEffect
    {
        public HideTabLabelsEffect()
            : base($"AppEffects.{nameof(HideTabLabelsEffect)}")
        {
        }
    }
}
