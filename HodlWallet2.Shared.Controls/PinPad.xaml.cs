using System;
using Xamarin.Forms;

namespace HodlWallet2.Shared.Controls
{
    public partial class PinPad : ContentView
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.CreateAttached(
                "TitleText", 
                typeof(string), 
                typeof(PinPad), 
                default(string)
            );

        public string TitleText
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        
        public static readonly BindableProperty HeaderProperty =
            BindableProperty.CreateAttached(
                "HeaderText", 
                typeof(string), 
                typeof(PinPad), 
                default(string)
            );

        public string HeaderText
        {
            get => (string) GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        
        public static readonly BindableProperty WarningProperty =
            BindableProperty.CreateAttached(
                "WarningText", 
                typeof(string), 
                typeof(PinPad), 
                default(string)
            );

        public string WarningText
        {
            get => (string) GetValue(WarningProperty);
            set => SetValue(WarningProperty, value);
        }

        public PinPad()
        {
            InitializeComponent();
            
            lblTitle.SetBinding(Label.TextProperty, new Binding(nameof(TitleText), source: this));
            lblHeader.SetBinding(Label.TextProperty, new Binding(nameof(HeaderText), source: this));
            lblWarning.SetBinding(Label.TextProperty, new Binding(nameof(WarningText), source: this));
        }
    }
}