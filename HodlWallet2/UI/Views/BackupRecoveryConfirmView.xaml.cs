using Xamarin.Forms;

using HodlWallet2.Core.ViewModels;

namespace HodlWallet2.UI.Views
{
    public partial class BackupRecoveryConfirmView : ContentPage
    {
        BackupRecoveryConfirmViewModel _ViewModel => (BackupRecoveryConfirmViewModel) BindingContext;

        public BackupRecoveryConfirmView(string[] mnemonic)
        {
            InitializeComponent();

            _ViewModel.Mnemonic = mnemonic;

            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<BackupRecoveryConfirmViewModel>(this, "NavigateToRootView", NavigateToRootView);
        }

        void NavigateToRootView(BackupRecoveryConfirmViewModel _)
        {
            // If the recovery was launched later...
            if (Navigation.ModalStack.Count > 0)
            {
                Navigation.PopModalAsync();

                // NOTE if we want to go to home we can add this line.
                // MessagingCenter.Send(this, "ChangeCurrentPageTo", RootView.Tabs.Home);

                return;
            }

            // First time launching recovery we finish the account creation!
            Navigation.PushAsync(new RootView());
        }
    }
}
