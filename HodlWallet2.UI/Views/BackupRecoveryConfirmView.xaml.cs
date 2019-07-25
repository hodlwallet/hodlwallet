using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;

using HodlWallet2.Locale;
using MvvmCross.Forms.Views;

namespace HodlWallet2.UI.Views
{
    public partial class BackupRecoveryConfirmView : MvxContentPage<BackupRecoveryConfirmViewModel>
    {
        public BackupRecoveryConfirmView()
        {
            InitializeComponent();
        }
    }
}
