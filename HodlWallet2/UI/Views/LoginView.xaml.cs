using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet2.Core.ViewModels;

namespace HodlWallet2.UI.Views
{
    public partial class LoginView : ContentPage
    {
        uint _AnimationTimeput = 50;

        Color _DigitOnColor => (Color)Application.Current.Resources["InputPinOn"];
        Color _DigitOffColor => (Color)Application.Current.Resources["InputPinOff"];

        public LoginView()
        {
            InitializeComponent();

            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<LoginViewModel, int>(this, "DigitAdded", DigitAdded);
            MessagingCenter.Subscribe<LoginViewModel, int>(this, "DigitRemoved", DigitRemoved);
            MessagingCenter.Subscribe<LoginViewModel>(this, "IncorrectPinAnimation", async (obj) => await IncorrectPinAnimation(obj));
            MessagingCenter.Subscribe<LoginViewModel>(this, "NavigateToRootView", NavigateToRootView);
            MessagingCenter.Subscribe<LoginViewModel>(this, "ResetPin", ResetPin);
        }

        void DigitAdded(LoginViewModel _, int index)
        {
            Debug.WriteLine($"[SubscribeToMessage][DigitAdded] Add digit: {index}");

            ColorDigitTo(index, _DigitOnColor);
        }

        void DigitRemoved(LoginViewModel _, int index)
        {
            Debug.WriteLine($"[SubscribeToMessage][DigitRemoved] Remove digit: {index}");

            ColorDigitTo(index, _DigitOffColor);
        }

        async Task IncorrectPinAnimation(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][IncorrectPinAnimation]");

            // Shake ContentView Re-Enter PIN
            await InputGrid.TranslateTo(-15, 0, _AnimationTimeput);
            await InputGrid.TranslateTo(15, 0, _AnimationTimeput);
            await InputGrid.TranslateTo(-10, 0, _AnimationTimeput);
            await InputGrid.TranslateTo(10, 0, _AnimationTimeput);
            await InputGrid.TranslateTo(-5, 0, _AnimationTimeput);
            await InputGrid.TranslateTo(5, 0, _AnimationTimeput);

            InputGrid.TranslationX = 0;

            await Task.Delay(500);
        }

        void NavigateToRootView(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][NavigateToRootView]");

            Navigation.PushAsync(new RootView());
        }

        void ResetPin(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][ResetPin]");

            ColorDigitTo(Pin1, _DigitOffColor);
            ColorDigitTo(Pin2, _DigitOffColor);
            ColorDigitTo(Pin3, _DigitOffColor);
            ColorDigitTo(Pin4, _DigitOffColor);
            ColorDigitTo(Pin5, _DigitOffColor);
            ColorDigitTo(Pin6, _DigitOffColor);
        }

        void ColorDigitTo(int index, Color color)
        {
            var pin = (BoxView)FindByName($"Pin{index}");

            ColorDigitTo(pin, color);
        }

        void ColorDigitTo(BoxView pin, Color color)
        {
            pin.Color = color;
        }
    }
}
