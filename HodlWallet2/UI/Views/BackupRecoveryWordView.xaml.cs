using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;

namespace HodlWallet2.UI.Views
{
    public partial class BackupRecoveryWordView : ContentPage
    {
        public BackupRecoveryWordView()
        {
            InitializeComponent();

            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<BackupRecoveryWordViewModel, string[]>(this, "NavigateToBackupRecoveryConfirmView", async (vm, mnemonic) => await NavigateToBackupRecoveryConfirmView(vm, mnemonic));
        }

        async Task NavigateToBackupRecoveryConfirmView(BackupRecoveryWordViewModel _, string[] mnemonic)
        {
            Debug.WriteLine($"[NavigateToBackupRecoveryConfirmView] About to write mnemonic: {string.Join(" ", mnemonic)}");

            await Navigation.PushAsync(new BackupRecoveryConfirmView(mnemonic));
        }
    }
}
