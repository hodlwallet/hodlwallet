using System.Threading.Tasks;
using System.Linq;

using MvvmCross.ViewModels;

using HodlWallet2.Core.Utils;
using HodlWallet2.Core.ViewModels;
using HodlWallet2.Core.Services;

using System.Diagnostics;

namespace HodlWallet2.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            // NOTE: Use this code to simulate first experience.
            //SecureStorageProvider.RemoveAll();

            // NOTE: Use this code to simulate login with a predictable stuff
            // SecureStorageProvider.SetPin("111111");
            // SecureStorageProvider.SetMnemonic("abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon about");
            // SecureStorageProvider.SetNetwork("testnet");

            //SecureStorageProvider.RemoveAll();
            //Process.GetCurrentProcess().Kill();

            SecureStorageProvider.LogSecureStorageKeys();

            if (SecureStorageProvider.HasPin() && SecureStorageProvider.HasMnemonic())
            {
                RegisterAppStart<LoginViewModel>();
            }
            else
            {
                RegisterAppStart<OnboardViewModel>();
            }
        }

        /// <summary>
        /// Do any UI bound startup actions here
        /// </summary>
        public override Task Startup()
        {
            return base.Startup();
        }

        /// <summary>
        /// If the application is restarted (eg primary activity on Android 
        /// can be restarted) this method will be called before Startup
        /// is called again
        /// </summary>
        public override void Reset()
        {
            base.Reset();
        }
    }
}