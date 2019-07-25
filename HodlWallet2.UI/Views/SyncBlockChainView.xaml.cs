using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HodlWallet2.Core.ViewModels;
using MvvmCross.Forms.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HodlWallet2.UI.Views
{
    public partial class SyncBlockChainView : MvxContentPage<SyncBlockChainViewModel>
    {
        public SyncBlockChainView()
        {
            InitializeComponent();
        }
    }
}