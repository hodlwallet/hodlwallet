using System;
using Xamarin.Forms;

namespace HodlWallet2.Core.Behaviors
{
    public class PaintBoxViewBehavior : Behavior<Button>
    {
        protected override void OnAttachedTo(Button bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Clicked += BindableOnClicked;
        }

        protected override void OnDetachingFrom(Button bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Clicked -= BindableOnClicked;
        }

        private void BindableOnClicked(object sender, EventArgs e)
        {
            
        }
    }
}