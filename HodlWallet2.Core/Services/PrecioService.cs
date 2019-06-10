using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Models;

using Refit;
using MvvmCross;

namespace HodlWallet2.Core.Services
{
    public class PrecioService
    {
        readonly IPrecioService _PrecioApi;
        string HOST = "https://precio.bitstop.co";

        public PrecioService() 
        {
            _PrecioApi = RestService.For<IPrecioService>(HOST);
        }

        public async Task<List<CurrencyEntity>> GetRates() => await _PrecioApi.GetRates();

        public async Task<List<CurrencyEntity>> GetTestnetRates() => await _PrecioApi.GetTestnetRates();

        public async Task<FeeEntity> GetFees() => await _PrecioApi.GetFees();
    }
}
