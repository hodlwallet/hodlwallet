using System;
using HodlWallet2.Droid.Renderers;
using HodlWallet2.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;

[assembly: ExportRenderer(typeof(FeeSlider), typeof(FeeSliderRenderer))]
namespace HodlWallet2.Droid.Renderers
{
    public class FeeSliderRenderer : SliderRenderer
    {
        public FeeSliderRenderer(Context context) : base(context)
        { 
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);
        }
    }
}
