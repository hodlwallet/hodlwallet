using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using HodlWallet2.UI.Renderers;
using HodlWallet2.iOS.Renderers;

[assembly: ExportRenderer(typeof(FeeSlider), typeof(FeeSliderRenderer))]
namespace HodlWallet2.iOS.Renderers
{
    public class FeeSliderRenderer : SliderRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);
        }
    }
}
