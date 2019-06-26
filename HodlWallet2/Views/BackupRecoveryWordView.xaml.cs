using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using Xamarin.Forms;
using MvvmCross.Forms.Views;
using HodlWallet2.Core.Interfaces;

namespace HodlWallet2.Views
{
    public partial class BackupRecoveryWordView : MvxContentPage<BackupRecoveryWordViewModel>
    {
        public BackupRecoveryWordView()
        {
            InitializeComponent();
        }
    }
}
