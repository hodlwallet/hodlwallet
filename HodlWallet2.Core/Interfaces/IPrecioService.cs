using System;
using System.Collections.Generic;
using Refit;
using System.Threading.Tasks;
using HodlWallet2.Core.Models;

namespace HodlWallet2.Core.Interfaces
{
    public interface IPrecioService
    {
        [Get("/hodl/rates.json")]
        Task<List<CurrencyEntity>> GetRates();

        [Get("/hodl.staging/rates.json")]
        Task<List<CurrencyEntity>> GetTestnetRates();

        [Get("/hodl/fee-estimator.json")]
        Task<FeeEntity> GetFees();
    }
}
