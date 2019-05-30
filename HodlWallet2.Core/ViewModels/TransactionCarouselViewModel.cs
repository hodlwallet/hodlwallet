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
    public class TransactionCarouselViewModel : BaseViewModel<Tuple<ObservableCollection<Transaction>, int>>
    {
        public ObservableCollection<Transaction> Transactions;
        public int CurrentIndex;

        public TransactionCarouselViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
        }

        public override void Prepare(Tuple<ObservableCollection<Transaction>, int> parameter)
        {
            Transactions = parameter.Item1;
            CurrentIndex = parameter.Item2;
        }

        private async Task Close()
        {
            await NavigationService.Close(this);
        }
    }
}
