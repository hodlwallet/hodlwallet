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
        Task<Dictionary<string, CurrencyEntity>> GetRates();

        [Get("/hodl.staging/rates.json")]
        Task<Dictionary<string, CurrencyEntity>> GetTestnetRates();

        [Get("/hodl/fee-estimator.json")]
        Task<>
    }
}
