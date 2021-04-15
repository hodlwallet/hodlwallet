//
// FontsDemoView.xaml.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 
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
using System.Collections.Generic;

using Xamarin.Forms;

namespace HodlWallet.UI.Views.Demos
{
    public partial class FontsDemoView : ContentPage
    {
        string[] _FontNames = {
            "Sans-Regular",
            "Sans-Bold",
            "Sans-Italic",
            "Sans-BoldItalic",
            "Mono-Regular",
            "Mono-Bold"
        };
        string[] _FontTitleNames = {
            "Regular",
            "Bold",
            "Italic",
            "Bold Italic",
            "Monospaced",
            "Monospaced Bold"
        };
        List<string> _Fonts = new List<string> { };
        int _CurrentlySelected;

        public static readonly BindableProperty CurrentFontTitleProperty = BindableProperty.CreateAttached(
                 nameof(CurrentFontTitle),
                 typeof(string),
                 typeof(FontsDemoView),
                 default(string)
         );

        public string CurrentFontTitle
        {
            get => (string)GetValue(CurrentFontTitleProperty);
            set => SetValue(CurrentFontTitleProperty, value);
        }

        public FontsDemoView()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(CurrentFontTitle))
            {
                ChangeCurrentFontTitle(CurrentFontTitle);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            foreach (var fontName in _FontNames)
            {
                var appFontName = (string)(OnPlatform<string>)Application.Current.Resources[fontName];

                _Fonts.Add(appFontName);
            }

            CurrentFontTitle = _FontTitleNames[_CurrentlySelected];
        }

        void ChangeCurrentFontTitle(string content)
        {
            FontTitleLabel.Text = content;
        }

        void ChangeFontUp(object sender, EventArgs e)
        {
            _CurrentlySelected += 1;

            if (_CurrentlySelected > 5)
                _CurrentlySelected = 0;

            ContentLabel.FontFamily = _Fonts[_CurrentlySelected];
            CurrentFontTitle = _FontTitleNames[_CurrentlySelected];
        }

        void ChangeFontDown(object sender, EventArgs e)
        {
            _CurrentlySelected -= 1;

            if (_CurrentlySelected < 0)
                _CurrentlySelected = 5;

            ContentLabel.FontFamily = _Fonts[_CurrentlySelected];
            CurrentFontTitle = _FontTitleNames[_CurrentlySelected];
        }
    }
}
