using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Services;
using HodlWallet2.Core.Models;
using HodlWallet2.Core.Utils;
using Liviano.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using Newtonsoft.Json;

namespace HodlWallet2.Core.ViewModels
{
    public class TransactionCollectionViewModel : BaseViewModel
    {
        public TransactionCollectionViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            // TODO
        }
    }
}
