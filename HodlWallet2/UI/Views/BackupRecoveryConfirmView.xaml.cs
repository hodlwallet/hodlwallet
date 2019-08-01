using Xamarin.Forms;

using HodlWallet2.Core.ViewModels;
using System.Threading.Tasks;

namespace HodlWallet2.UI.Views
{
    public partial class BackupRecoveryConfirmView : ContentPage
    {
        public BackupRecoveryConfirmView(string[] mnemonic)
        {
            InitializeComponent();

            var vm = (BackupRecoveryConfirmViewModel)BindingContext;

            vm.Mnemonic = mnemonic;

            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<BackupRecoveryConfirmViewModel>(this, "NavigateToRootView", async (vm) => await NavigateToRootView(vm));
        }

        async Task NavigateToRootView(BackupRecoveryConfirmViewModel _)
        {
            await Navigation.PushAsync(new RootView());
        }
    }
}
