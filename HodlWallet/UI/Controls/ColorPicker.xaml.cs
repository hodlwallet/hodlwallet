using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPicker
    {
        public static readonly BindableProperty ButtonColorSelectedProperty
            = BindableProperty.Create(nameof(ButtonColorSelected), typeof(Color), typeof(ColorPicker), Color.White);

        public Color ButtonColorSelected
        {
            get => (Color)GetValue(ButtonColorSelectedProperty);

            set => SetValue(ButtonColorSelectedProperty, value);
        }

        public ColorPicker()
        {
            InitializeComponent();
        }

        public void ColorButtonClicked(object sender, EventArgs e)
        {
            Button pressed = sender as Button;
            ButtonColorSelected = pressed.BackgroundColor;
        }
    }
}