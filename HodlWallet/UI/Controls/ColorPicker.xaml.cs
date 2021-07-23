using System;
using System.Collections.Generic;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HodlWallet.Core.Utils;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPicker//: INotifyPropertyChanged
    {
        public static readonly BindableProperty ButtonColorSelectedProperty
            = BindableProperty.Create(nameof(ButtonColorSelected), 
                typeof(Color), 
                typeof(ColorPicker), 
                Color.White,
                propertyChanged: OnEventColorChanged);

        public static readonly BindableProperty ButtonColorCodeProperty
            = BindableProperty.Create(nameof(ColorCode),
                typeof(string),
                typeof(ColorPicker),
                Constants.DEFAULT_ACCOUNT_COLOR_CODE);

        public static void OnEventColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ColorPicker colorPicker = bindable as ColorPicker;
            foreach (var element in colorPicker.Children)
            {
                if (element is Button button)
                {
                    if (element.BackgroundColor == colorPicker.ButtonColorSelected)
                    {
                        button.BorderColor = (Color)Application.Current.Resources["ColorPickerSelected"];
                        colorPicker.ColorCode = button.Text;
                    }
                    else
                    {
                        button.BorderColor = Color.Transparent;
                    }
                }
            }
        }
         
        public static Color[] colorPickerControlList =
        {
            (Color)Application.Current.Resources["ColorPicker0" ],
            (Color)Application.Current.Resources["ColorPicker1" ],
            (Color)Application.Current.Resources["ColorPicker2" ],
            (Color)Application.Current.Resources["ColorPicker3" ],
            (Color)Application.Current.Resources["ColorPicker4" ],
            (Color)Application.Current.Resources["ColorPicker5" ],
            (Color)Application.Current.Resources["ColorPicker6" ],
            (Color)Application.Current.Resources["ColorPicker7" ],
            (Color)Application.Current.Resources["ColorPicker8" ],
            (Color)Application.Current.Resources["ColorPicker9" ],
            (Color)Application.Current.Resources["ColorPicker10"],
            (Color)Application.Current.Resources["ColorPicker11"],
            (Color)Application.Current.Resources["ColorPicker12"],
            (Color)Application.Current.Resources["ColorPicker13"],
            (Color)Application.Current.Resources["ColorPicker14"],
            (Color)Application.Current.Resources["ColorPicker15"],
            (Color)Application.Current.Resources["ColorPicker16"],
            (Color)Application.Current.Resources["ColorPicker17"]
        };

        public IList<Color> ColorPickerControlList { get => colorPickerControlList; }

        public Color ButtonColorSelected
        {
            get => (Color)GetValue(ButtonColorSelectedProperty);

            set => SetValue(ButtonColorSelectedProperty, value);
        }

        public string ColorCode
        {
            get => (string)GetValue(ButtonColorCodeProperty);

            set => SetValue(ButtonColorCodeProperty, value);
        }

        public ColorPicker()
        {
            InitializeComponent();
        }

        public void ColorButtonClicked(object sender, EventArgs e)
        {
            Button pressed = sender as Button;
            ButtonColorSelected = pressed.BackgroundColor;
            ColorCode = pressed.Text;
        }
    }
}