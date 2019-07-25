using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;
using Serilog;

using HodlWallet2.Locale;
using HodlWallet2.Core.Utils;
using MvvmCross.Forms.Views;

namespace HodlWallet2.UI.Views
{
    public partial class BackupView : MvxContentPage<BackupViewModel>
    {
        public BackupView()
        {
            InitializeComponent();
        }

        private void BackupButton_Clicked(object sender, EventArgs e)
        {
//            TODO: Move this to view model.
//            Navigation.PushAsync(new BackupRecoveryWordView(1, mnemonic));
//            _Logger.Information("Backup button clicked.");
        }
    }
}
