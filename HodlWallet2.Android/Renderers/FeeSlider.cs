using System;
using HodlWallet2.Droid.Renderers;
using HodlWallet2.UI.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Widget;

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

            if (e.NewElement == null) return;

            ShapeDrawable th = new ShapeDrawable(new OvalShape());

            th.SetIntrinsicWidth(50);
            th.SetIntrinsicHeight(50);
            th.SetColorFilter(Android.Graphics.Color.White, Android.Graphics.PorterDuff.Mode.SrcOver);

            Control.SetThumb(th);
        }
    }
}
