using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet2.Core.Services;
using HodlWallet2.UI.Views;
using HodlWallet2.Core.Interfaces;

namespace HodlWallet2.UI
{
    public partial class App : Application
    {
        IWalletService _WalletService => DependencyService.Get<IWalletService>();

        public App()
        {
            InitializeComponent();

            // Register services
            DependencyService.Register<IWalletService>();
            DependencyService.Register<IShareIntent>();
            DependencyService.Register<IPermissions>();

            if (UserDidSetup())
            {
                MainPage = new NavigationPage(new RootView());
            }
            else
            {
                MainPage = new NavigationPage(new OnboardView());
            }
        }

        protected override void OnStart()
        {
            // NOTE You might think, why not move this forward?
            // the init code that inserts the logger into
            // WalletService is only run after the custructor
            // and only after all the platforms init
            Task.Run(_WalletService.InitializeWallet);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        bool UserDidSetup()
        {
            return SecureStorageService.HasPin()
                && SecureStorageService.HasMnemonic()
                && SecureStorageService.HasSeedBirthday()
                && SecureStorageService.HasWalletId()
                && SecureStorageService.HasNetwork();
        }
    }
}
