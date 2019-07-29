using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HodlWallet2.UI.Controls
{
    public class MoneyLabel : StackLayout
    {
        object _Lock = new object();

        Animation _Animation = new Animation();

        const string ALLOWED_DIGITS = "9876543210";
        const string ALLOWED_SYMBOLS = ",.-";
        const string ALLOWED_CHARACTERS = ALLOWED_DIGITS + ALLOWED_SYMBOLS;

        const uint ANIMATION_DURATION_PER_CHARACTER = 250;

        [TypeConverter(typeof(FontAttributesConverter))]
        public FontAttributes FontAttributes
        {
            get { return (FontAttributes)GetValue(FontAttributesProperty); }
            set { SetValue(FontAttributesProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MoneyLabel), defaultValueCreator: bindable => new Label().TextColor);

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(MoneyLabel), defaultValueCreator: bindable => new Label().FontFamily);

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(MoneyLabel), -1.0,
               defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Default, typeof(Label)));

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(MoneyLabel), defaultValueCreator: bindable => new Label().FontAttributes);

        public static readonly BindableProperty TextProperty = BindableProperty.CreateAttached(
                nameof(Text),
                typeof(string),
                typeof(MoneyLabel),
                default(string)
        );

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public MoneyLabel()
        {
            Orientation = StackOrientation.Horizontal;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Text))
            {
                ValidateText();

                _ = ChangeTextAsync();

                LaunchScrollingAnimations();
            }

            if (propertyName == nameof(FontFamily))
            {
                FontFamily = ToLabelFontFamily(FontFamily);

                ChangeFontFamily();
            }

            if (propertyName == nameof(FontSize))
            {
                ChangeFontSize();
            }

            if (propertyName == nameof(TextColor))
            {
                ChangeTextColor();
            }

            if (propertyName == nameof(FontAttributes))
            {
                ChangeTextFontAttributes();
            }
        }

        void ChangeTextFontAttributes()
        {
            foreach (var child in Children)
            {
                var moneyDigit = (MoneyDigit)child;

                foreach (var digit in ((StackLayout)moneyDigit.Content).Children)
                {
                    var label = (Label)digit;

                    if (label.FontAttributes == FontAttributes)
                        break;

                    label.FontAttributes = FontAttributes;
                }
            }
        }

        void ChangeTextColor()
        {
            foreach (var child in Children)
            {
                var moneyDigit = (MoneyDigit)child;

                foreach (var digit in ((StackLayout)moneyDigit.Content).Children)
                {
                    var label = (Label)digit;

                    if (label.TextColor == TextColor)
                        break;

                    label.TextColor = TextColor;
                }
            }
        }

        void ChangeFontFamily()
        {
            foreach (var child in Children)
            {
                var moneyDigit = (MoneyDigit)child;

                foreach (var digit in ((StackLayout)moneyDigit.Content).Children)
                {
                    var label = (Label)digit;

                    if (label.FontFamily == FontFamily)
                        break;

                    label.FontFamily = FontFamily;
                }
            }
        }

        void ChangeFontSize()
        {
            foreach (var child in Children)
            {
                var moneyDigit = (MoneyDigit)child;

                moneyDigit.HeightRequest = FontSize;

                foreach (var digit in ((StackLayout)moneyDigit.Content).Children)
                {
                    var label = (Label)digit;

                    if (label.FontSize.Equals(FontSize))
                        break;

                    label.FontSize = FontSize;
                }
            }
        }

        string ToLabelFontFamily(string fontFamily)
        {
            var lbl = new Label
            {
                FontFamily = fontFamily
            };

            return lbl.FontFamily;
        }

        /// <summary>
        /// Text can only be a decimal
        /// </summary>
        void ValidateText()
        {
            // This will fire a format exception
            if (!string.IsNullOrEmpty(Text)) decimal.Parse(Text);
        }

        async Task ChangeTextAsync()
        {
            if (Children.Count == 0)
                AddNewText();
            else
                await ModifyText();
        }

        void AddNewText()
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;

            foreach (var textChar in Text)
            {
                AddNewChar(textChar);
            }
        }

        async Task ModifyText()
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                lock (_Lock)
                {
                    Children.Clear();
                }

                return;
            }

            // We add the new ones
            bool weAdded = false;

            int childrenCount = Children.Count();
            if (Text.Length > childrenCount)
            {
                for (int i = childrenCount, count = Text.Length; i < count; i++)
                {
                    AddNewChar(Text[i]);

                    if (!weAdded) weAdded = true;
                }
            }
            else
            {
                lock (_Lock)
                {
                    while (Children.Count() != Text.Length)
                    {
                        var child = Children[Children.Count() - 1];
                        Children.Remove(child);
                    }
                }
            }

            if (weAdded)
            {
                LayoutChanged += async (object sender, EventArgs args) =>
                {
                    if (weAdded) await _ModifyText();
                };

                return;
            }

            await _ModifyText();
        }

        async Task _ModifyText()
        {
            // Now we replace the old chars
            for (int i = 0, count = Text.Length; i < count; i++)
            {
                // Sometimes shit happens...
                if (i > Children.Count - 1) continue;

                // It's a character change!
                var label = ((MoneyDigit)Children[i]).CurrentLabel;
                char oldChar = label.Text[0];
                char newChar = Text[i];

                if (oldChar != newChar)
                    await FlipChars(oldChar, newChar, i);
            }
        }

        void LaunchScrollingAnimations()
        {
            var fullAnimationDuration = (uint)(Text.Length * ANIMATION_DURATION_PER_CHARACTER);

            bool animationAdded = false;
            double position = 0.0;
            for (int i = Text.Length - 1; i >= 0; i--)
            {
                if (i > Children.Count - 1) continue;

                var moneyDigit = (MoneyDigit)Children[i];

                if (moneyDigit.CurrentLabel is null) continue;

                var startAt = position;
                var endAt = startAt + (ANIMATION_DURATION_PER_CHARACTER / (float)fullAnimationDuration);

                if (endAt >= 1.0) endAt = 1.0;

                var moneyDigitAnimation = moneyDigit.GetScrollingToCurrentAnimation();

                if (moneyDigitAnimation != null)
                {
                    _Animation.Add(startAt, endAt, moneyDigitAnimation);
                    if (!animationAdded) animationAdded = true;
                }

                position = endAt;
            }

            if (animationAdded)
            {
                _Animation.Commit(
                    owner: this,
                    name: "ChildrenAnimation",
                    length: fullAnimationDuration
                );
            }
        }

        async Task FlipChars(char oldChar, char newChar, int index)
        {
            if (oldChar == newChar) return;

            //int oldInt = int.Parse(oldChar.ToString());
            //int newInt = int.Parse(oldChar.ToString());

            var moneyDigit = (MoneyDigit)Children[index];
            var stack = (StackLayout)moneyDigit.Content;

            var label = (Label)stack.Children.FirstOrDefault((l) =>
            {
                return ((Label)l).Text == newChar.ToString();
            });

            moneyDigit.CurrentLabel = label;
        }

        void AddNewChar(char newChar)
        {
            lock (_Lock)
            {
                var moneyDigit = CreateNewChar(newChar);

                Children.Add(moneyDigit);
            }
        }

        MoneyDigit CreateNewChar(char newChar)
        {
            var scroll = new MoneyDigit
            {
                HeightRequest = FontSize,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                VerticalScrollBarVisibility = ScrollBarVisibility.Never,
                IsEnabled = false,
                Digit = newChar
            };

            var stack = new StackLayout();

            // TODO Pick a font
            var labels = ALLOWED_CHARACTERS.Select((chr) =>
                new Label
                {
                    FontFamily = FontFamily,
                    FontSize = FontSize,
                    FontAttributes = FontAttributes,
                    TextColor = TextColor,
                    Text = chr.ToString()
                }
            );

            Label chrLabel = null;
            foreach (var l in labels)
            {
                stack.Children.Add(l);

                if (l.Text == newChar.ToString())
                    chrLabel = (Label)stack.Children.Last();
            }

            if (chrLabel is null)
                throw new ArgumentException($"[CreateNewChar] Failed to add digit {newChar}");

            scroll.Content = stack;

            scroll.LayoutChanged += async (object sender, EventArgs e) =>
            {
                var s = (MoneyDigit)sender;

                await s.ScrollToAsync(chrLabel, ScrollToPosition.Center, false);

                s.CurrentLabel = chrLabel;
            };

            return scroll;
        }
    }

    class MoneyDigit : ScrollView
    {
        public static readonly BindableProperty DigitProperty =
            BindableProperty.Create(nameof(Digit), typeof(char),
                typeof(MoneyDigit),
                default(char));

        public char Digit
        {
            get => (char)GetValue(DigitProperty);
            set => SetValue(DigitProperty, value);
        }

        public static readonly BindableProperty CurrentLabelProperty =
            BindableProperty.Create(nameof(CurrentLabel), typeof(Label),
                typeof(MoneyDigit),
                default(Label));

        public Label CurrentLabel
        {
            get => (Label)GetValue(CurrentLabelProperty);
            set => SetValue(CurrentLabelProperty, value);
        }

        public Animation GetScrollingToCurrentAnimation()
        {
            if (CurrentLabel is null) return null;

            Point point = GetScrollPositionForElement(CurrentLabel, ScrollToPosition.Center);

            var mainAnimation = new Animation();

            var animation = new Animation(
                callback: async y => await ScrollToAsync(point.X, y, animated: false),
                start: ScrollY,
                end: point.Y,
                easing: Easing.CubicInOut
            );

            mainAnimation.Add(0.0, 1.0, animation);

            // TODO Idea, the animation should be stacked... maybe going label to label...
            //var animation1 = new Animation(
            //    callback: async y => await ScrollToAsync(point.X, y, animated: false),
            //    start: ScrollY,
            //    end: ScrollY + (point.Y / 8)
            //);

            //var animation2 = new Animation(
            //    callback: async y => await ScrollToAsync(point.X, y, animated: false),
            //    start: ScrollY + (point.Y / 8),
            //    end: (ScrollY + (point.Y / 8)) + point.Y / 4
            //);

            //var animation3 = new Animation(
            //    callback: async y => await ScrollToAsync(point.X, y, animated: false),
            //    start: ScrollY + (point.Y / 2),
            //    end: ((ScrollY + (point.Y / 8)) + point.Y / 4) + point.Y / 2
            //);

            //mainAnimation.Add(0, 0.4, animation1);
            //mainAnimation.Add(0.5, 0.7, animation2);
            //mainAnimation.Add(0.8, 1.0, animation3);


            return mainAnimation;
        }
    }
}