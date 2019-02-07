using System;
using System.Collections.Generic;

using Xamarin.Forms;

using HodlWallet2.Locale;
using HodlWallet2.ViewModels;

namespace HodlWallet2.Views
{
    public partial class BackupRecoveryConfirmView : ContentPage
    {
        public BackupConfirmViewModel ViewModel { get => BindingContext as BackupConfirmViewModel; }
        private BackupConfirmViewModel _ViewModel;

        public BackupRecoveryConfirmView(BackupConfirmViewModel viewModel)
        {
            InitializeComponent();
            SetLabels();
            _ViewModel = viewModel;
            BindingContext = _ViewModel;
        }

        public BackupRecoveryConfirmView()
        {
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            Title = LocaleResources.Backup_title;
            Header.Text = LocaleResources.ConfirmBackup_header;
        }

    }
}
