using System.Threading.Tasks;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Utils;
using HodlWallet2.Core.ViewModels;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace HodlWallet2.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            // NOTE: Use this code to simulate first experience.
            //SecureStorageProvider.RemoveAll();

            if (SecureStorageProvider.HasPassword() && SecureStorageProvider.HasMnemonic())
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