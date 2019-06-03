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
        ObservableCollection<Transaction> _Transactions;
        public ObservableCollection<Transaction> Transactions
        {
            get => _Transactions;
            set => SetProperty(ref _Transactions, value);
        }

        int _CurrentIndex;
        public int CurrentIndex
        {
            get => _CurrentIndex;
            set => SetProperty(ref _CurrentIndex, value);
        }

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

        async Task Close()
        {
            await NavigationService.Close(this);
        }
    }
}
