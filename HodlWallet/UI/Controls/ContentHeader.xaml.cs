//
// ContentHeader.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2022 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentHeader : ScrollView
    {
        // Color
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
            nameof(Color),
            typeof(Color),
            typeof(ContentHeader),
            (Color)Application.Current.Resources["FgSuccess"]
        );
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Fonts
        public static readonly BindableProperty ImageFontFamilyProperty = BindableProperty.Create(
            nameof(ImageFontFamily),
            typeof(string),
            typeof(ContentHeader),
            "MaterialIcons-Regular"
        );
        public string ImageFontFamily
        {
            get { return (string)GetValue(ImageFontFamilyProperty); }
            set { SetValue(ImageFontFamilyProperty, value); }
        }

        public static readonly BindableProperty DescriptionFontFamilyProperty = BindableProperty.Create(
            nameof(DescriptionFontFamily),
            typeof(string),
            typeof(ContentHeader),
            (string)Application.Current.Resources["Sans-Regular"]
        );
        public string DescriptionFontFamily
        {
            get { return (string)GetValue(DescriptionFontFamilyProperty); }
            set { SetValue(DescriptionFontFamilyProperty, value); }
        }

        public static readonly BindableProperty TitleFontFamilyProperty = BindableProperty.Create(
            nameof(TitleFontFamily),
            typeof(string),
            typeof(ContentHeader),
            (string)Application.Current.Resources["Sans-Bold"]
        );
        public string TitleFontFamily
        {
            get { return (string)GetValue(TitleFontFamilyProperty); }
            set { SetValue(TitleFontFamilyProperty, value); }
        }

        // FontSize
        public static readonly BindableProperty DescriptionFontSizeProperty = BindableProperty.Create(
            nameof(DescriptionFontSize),
            typeof(double),
            typeof(ContentHeader),
            Device.GetNamedSize(NamedSize.Caption, typeof(Label))
        );

        [TypeConverter(typeof(FontSizeConverter))]
        public double DescriptionFontSize
        {
            get { return (double)GetValue(DescriptionFontSizeProperty); }
            set { SetValue(DescriptionFontSizeProperty, value); }
        }

        // Glyph
        public static readonly BindableProperty GlyphProperty = BindableProperty.Create(
            nameof(Glyph),
            typeof(string),
            typeof(ContentHeader),
            "question_mark"
        );
        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        // Text
        public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(
            nameof(TitleText),
            typeof(string),
            typeof(ContentHeader),
            string.Empty
        );
        public string TitleText
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        public static readonly BindableProperty DescriptionTextProperty = BindableProperty.Create(
            nameof(DescriptionText),
            typeof(string),
            typeof(ContentHeader),
            string.Empty
        );
        public string DescriptionText
        {
            get { return (string)GetValue(DescriptionTextProperty); }
            set { SetValue(DescriptionTextProperty, value); }
        }

        // Tapped
        public static readonly BindableProperty DescriptionTappedCommandProperty = BindableProperty.Create(
            nameof(DescriptionTappedCommand),
            typeof(ICommand),
            typeof(ContentHeader),
            null
        );
        public ICommand DescriptionTappedCommand
        {
            get { return (ICommand)GetValue(DescriptionTappedCommandProperty); }
            set { SetValue(DescriptionTappedCommandProperty, value); }
        }

        // Padding
        public static readonly BindableProperty DescriptionPaddingProperty = BindableProperty.Create(
            nameof(DescriptionPadding),
            typeof(Thickness),
            typeof(ContentHeader),
            new Thickness(90, 0, 90, 0)
        );
        public Thickness DescriptionPadding
        {
            get { return (Thickness)GetValue(DescriptionPaddingProperty); }
            set { SetValue(DescriptionPaddingProperty, value); }
        }

        public ContentHeader()
        {
            InitializeComponent();
        }
    }
}