using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;

using HodlWallet2.Locale;
using HodlWallet2.ViewModels;
using MvvmCross.Forms.Views;

namespace HodlWallet2.Views
{
    public partial class BackupRecoveryConfirmView : MvxContentPage<BackupRecoveryConfirmViewModel>
    {
        public BackupRecoveryConfirmView()
        {
            InitializeComponent();
        }
    }
}
