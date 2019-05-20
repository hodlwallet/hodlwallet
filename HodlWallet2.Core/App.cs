using System.Threading.Tasks;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
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
            //TODO: Construct and register ILogger for the WalletService on Android
            
            
            RegisterAppStart<OnboardViewModel>();
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