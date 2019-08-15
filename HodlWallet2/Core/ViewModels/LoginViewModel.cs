using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

using HodlWallet2.Core.Services;
using HodlWallet2.UI.Locale;

using Xamarin.Forms;

namespace HodlWallet2.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        List<int> _Pin = new List<int>();
        object _Lock = new object();

        public ICommand DigitCommand { get; }
        public ICommand BackspaceCommand { get; }

        public string Header { get; } = LocaleResources.Pin_enter;

        public LoginViewModel()
        {
            DigitCommand = new Command<string>((s) => _ = AddDigit(int.Parse(s)));
            BackspaceCommand = new Command(RemoveDigit);
        }

        async Task AddDigit(int digit)
        {
            Debug.WriteLine($"[AddDigit] Adding: {digit}");

            // Digit has already being inputed
            if (_Pin.Count >= 6) return;

            lock (_Lock)
            {
                _Pin.Add(digit);
            }

            MessagingCenter.Send(this, "DigitAdded", _Pin.Count);

            // Digit is not complete, input more
            if (_Pin.Count != 6) return;

            // _Pin.Count == 6 now...
            // We're done inputting our PIN
            await Task.Delay(305);

            MessagingCenter.Send(this, "ResetPin");

            string input = string.Join(string.Empty, _Pin.ToArray());

            // Check if it's the pin
            if (SecureStorageService.GetPin() == input)
            {
                _Pin.Clear();

                Debug.WriteLine("[AddDigit] Logged in!");

                // DONE! We navigate to the root view model
                MessagingCenter.Send(this, "NavigateToRootView");

                return;
            }

            Debug.WriteLine($"[AddDigit] Incorrect PIN: {input}");

            // Sadly it's not the pin! We clear and launch an animation
            _Pin.Clear();

            MessagingCenter.Send(this, "IncorrectPinAnimation");
        }

        void RemoveDigit()
        {
            Debug.WriteLine("[RemoveDigit]");

            if (_Pin.Count <= 0) return;

            lock (_Lock)
            {
                _Pin.RemoveAt(_Pin.Count - 1);

                MessagingCenter.Send(this, "DigitRemoved", _Pin.Count + 1);
            }
        }
    }
}
