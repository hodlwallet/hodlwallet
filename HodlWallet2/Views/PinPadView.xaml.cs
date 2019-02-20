using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

using HodlWallet2.Utils;
using HodlWallet2.Core.ViewModels;
using MvvmCross.Forms.Views;

namespace HodlWallet2.Views
{
    public partial class PinPadView : MvxContentPage<PinPadViewModel>
    {
        private string _pin1 = string.Empty;
        private string _pin2 = string.Empty;
        public PinPadView()
        {
            InitializeComponent();
        }
        
        public async void OnPinTapped(object sender, EventArgs e)
        {
            if (carouselPin.Position == 0) // Enter PIN
            {
                if (sender is Button button && _pin1.Length < 6)
                {
                    _pin1 += Utils.Tags.GetTag(button);
                    PaintBoxView(Color.Orange, _pin1.Length);
                    if (_pin1.Length == 6)
                    {
                        carouselPin.Position = 1;
                    }
                }
            }
            else // Re-Enter PIN
            {
                if (sender is Button button && _pin2.Length < 6)
                {
                    _pin2 += Utils.Tags.GetTag(button);
                    PaintBoxView(Color.Orange, _pin2.Length + 6);
                    if (_pin2.Length == 6)
                    {
                        if (_pin1.Equals(_pin2))
                        {
                            // Success!
                        }
                        else
                        {
                            // Shake ContentView Re-Enter PIN
                            uint timeout = 50;
                            await carouselPin.TranslateTo(-15, 0, timeout);  
  
                            await carouselPin.TranslateTo(15, 0, timeout);  
  
                            await carouselPin.TranslateTo(-10, 0, timeout);  
  
                            await carouselPin.TranslateTo(10, 0, timeout);  
  
                            await carouselPin.TranslateTo(-5, 0, timeout);  
  
                            await carouselPin.TranslateTo(5, 0, timeout);  
  
                            carouselPin.TranslationX = 0;  
                            
                            await Task.Delay(500);
                            ClearBoxViews();
                            carouselPin.Position = 0;
                        }
                        _pin1 = string.Empty;
                        _pin2 = string.Empty;
                    }
                }
            }
        }

        private void ClearBoxViews()
        {
            var list = carouselPin.ItemsSource.Cast<ContentView>().ToList();
            foreach (var contentView in list)
            {
                foreach (Grid grid in contentView.Children)
                {
                    foreach (var boxView in grid.Children)
                    {
                        if (boxView.BackgroundColor != Color.Transparent)
                        {
                            var bxView = boxView as BoxView;
                            if (bxView != null)
                            {
                                bxView.Color = Color.White;
                            }
                        }
                    }
                }
            }            
        }

        private void PaintBoxView(Color fillColor, int boxViewNumber)
        {
            var list = carouselPin.ItemsSource.Cast<ContentView>().ToList();
            var grid = carouselPin.Position == 0 ? list[0].Content as Grid : list[1].Content as Grid;
            BoxView bxView1 = new BoxView();
            foreach (var boxView in grid.Children)
            {
                var bxView = boxView as BoxView;
                if (bxView != null)
                {
                    var tag = Utils.Tags.GetTag(bxView);
                    if (tag.Equals(boxViewNumber.ToString()))
                    {
                        bxView.Color = Color.Orange;
                        break;
                    }
                }
            }
        }

        public async void OnPrevious(object sender, EventArgs e)
        {
            carouselPin.Position = 0;
        }

    }
}
