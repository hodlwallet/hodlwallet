//
// PinPad.xaml.cs
//
// Copyright (c) 2019 HODL Wallet
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
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Internals;

using HodlWallet.Core.Utils;

namespace HodlWallet.UI.Controls
{
    public partial class PinPad : ContentView
    {
        private string _pin1 = string.Empty;
        private string _pin2 = string.Empty;

        public static readonly BindableProperty TitleProperty =
            BindableProperty.CreateAttached(
                "TitleText",
                typeof(string),
                typeof(PinPad),
                default(string)
            );

        public string TitleText
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                lblTitle.Text = value;
            }
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
            get => (string)GetValue(HeaderProperty);
            set
            {
                SetValue(HeaderProperty, value);
                lblHeader.Text = value;
            }
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
            get => (string)GetValue(WarningProperty);
            set
            {
                SetValue(WarningProperty, value);
                lblWarning.Text = value;
            }
        }

        public PinPad()
        {
            InitializeComponent();

            lblTitle.SetBinding(Label.TextProperty, new Binding(nameof(TitleText), source: this));
            lblHeader.SetBinding(Label.TextProperty, new Binding(nameof(HeaderText), source: this));
            lblWarning.SetBinding(Label.TextProperty, new Binding(nameof(WarningText), source: this));
        }

        /// <summary>Backing store for the Command bindable property.</summary>
        /// <remarks>To be added.</remarks>
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(PinPad),
            (object)null,
            BindingMode.OneWay,
            (BindableProperty.ValidateValueDelegate)null,
            (BindableProperty.BindingPropertyChangedDelegate)((bo, o, n) => ((PinPad)bo).OnCommandChanged()),
            (BindableProperty.BindingPropertyChangingDelegate)null,
            (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        /// <summary>Backing store for the CommandParameter bindable property.</summary>
        /// <remarks>To be added.</remarks>
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(PinPad),
            (object)null,
            BindingMode.OneWay,
            (BindableProperty.ValidateValueDelegate)null,
            (BindableProperty.BindingPropertyChangedDelegate)((bindable, oldvalue, newvalue) => ((PinPad)bindable).CommandCanExecuteChanged((object)bindable, EventArgs.Empty)),
            (BindableProperty.BindingPropertyChangingDelegate)null,
            (BindableProperty.CoerceValueDelegate)null,
            (BindableProperty.CreateDefaultValueDelegate)null);

        /// <summary>Gets or sets the command to invoke when the button is activated. This is a bindable property.</summary>
        /// <value>A command to invoke when the button is activated. The default value is <see langword="null" />.</value>
        /// <remarks>This property is used to associate a command with an instance of a button. This property is most often set in the MVVM pattern to bind callbacks back into the ViewModel. <see cref="P:Xamarin.Forms.VisualElement.IsEnabled" /> is controlled by the Command if set.</remarks>
        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(PinPad.CommandProperty);
            }
            set
            {
                this.SetValue(PinPad.CommandProperty, (object)value);
            }
        }

        /// <summary>Gets or sets the parameter to pass to the Command property. This is a bindable property.</summary>
        /// <value>A object to pass to the command property. The default value is <see langword="null" />.</value>
        /// <remarks>To be added.</remarks>
        public object CommandParameter
        {
            get
            {
                return this.GetValue(PinPad.CommandParameterProperty);
            }
            set
            {
                this.SetValue(PinPad.CommandParameterProperty, value);
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            ICommand command = this.Command;
            if (command == null)
                return;
            this.IsEnabledCore = command.CanExecute(this.CommandParameter);
        }

        private bool IsEnabledCore
        {
            set
            {
                this.SetValueCore(VisualElement.IsEnabledProperty, (object)value, SetValueFlags.None);
            }
        }

        private void OnCommandChanged()
        {
            if (this.Command != null)
            {
                this.Command.CanExecuteChanged += new EventHandler(this.CommandCanExecuteChanged);
                this.CommandCanExecuteChanged((object)this, EventArgs.Empty);
            }
            else
                this.IsEnabledCore = true;
        }

        void OnBackspaceTapped(object sender, EventArgs e)
        {
            // FIXME This code should not work like this, it's kindof embarasing how poorly coded this is, for now I just gonna follow the mess that's bellow - Igor.
            if (grdSetPin.IsVisible) // "Enter Pin" Ugg... horrible way to know where you at.
            {
                if (_pin1.Length == 0)
                    return;

                _pin1 = _pin1.Remove(_pin1.Length - 1);
                PaintBoxView(Color.White, _pin1.Length + 1);
            }
            else // "Reenter Pin"
            {
                if (_pin2.Length == 0)
                    return;

                _pin2 = _pin2.Remove(_pin2.Length - 1);
                PaintBoxView(Color.White, _pin2.Length + 1);
            }
        }

        public async void OnPinTapped(object sender, EventArgs e)
        {
            if (grdSetPin.IsVisible) // Enter PIN
            {
                if (sender is Button button && _pin1.Length < 6)
                {
                    _pin1 += Tags.GetTag(button);
                    PaintBoxView(Color.Orange, _pin1.Length);
                    if (_pin1.Length == 6)
                    {
                        grdSetPin.IsVisible = false;
                        grdReSetPin.IsVisible = true;
                    }
                }
            }
            else // Re-Enter PIN
            {
                if (sender is Button button && _pin2.Length < 6)
                {
                    _pin2 += Tags.GetTag(button);
                    PaintBoxView(Color.Orange, _pin2.Length + 6);
                    if (_pin2.Length == 6)
                    {
                        if (_pin1.Equals(_pin2))
                        {
                            // TODO: FIX POPUP STACK ERROR
                            // await Navigation.PushPopupAsync(new SetPin());
                            Command?.Execute(_pin1);
                        }
                        else
                        {
                            // Shake ContentView Re-Enter PIN
                            uint timeout = 50;
                            await grdReSetPin.TranslateTo(-15, 0, timeout);

                            await grdReSetPin.TranslateTo(15, 0, timeout);

                            await grdReSetPin.TranslateTo(-10, 0, timeout);

                            await grdReSetPin.TranslateTo(10, 0, timeout);

                            await grdReSetPin.TranslateTo(-5, 0, timeout);

                            await grdReSetPin.TranslateTo(5, 0, timeout);

                            grdReSetPin.TranslationX = 0;

                            await Task.Delay(500);
                            ClearBoxViews();
                            grdSetPin.IsVisible = true;
                            grdReSetPin.IsVisible = false;
                        }
                        _pin1 = string.Empty;
                        _pin2 = string.Empty;
                    }
                }
            }
        }

        private void ClearBoxViews()
        {
            foreach (var element in cntViewBoxes.Children)
            {
                if (element is Grid grid)
                {
                    foreach (var boxView in grid.Children)
                    {
                        if (boxView.BackgroundColor != Color.Transparent)
                        {
                            if (boxView is BoxView bxView)
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
            var grid = grdSetPin.IsVisible ? grdSetPin : grdReSetPin;
            foreach (var boxView in grid.Children)
            {
                var bxView = boxView as BoxView;
                if (bxView != null)
                {
                    var tag = Tags.GetTag(bxView);
                    if (tag.Equals(boxViewNumber.ToString()))
                    {
                        bxView.Color = fillColor;
                        break;
                    }
                }
            }
        }
    }
}