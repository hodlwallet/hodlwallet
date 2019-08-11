using System;
using System.Collections.Generic;
using Refit;
using System.Threading.Tasks;
using HodlWallet2.Core.Models;

namespace HodlWallet2.Core.Interfaces
{
    public interface IPrecioHttpService
    {
        [Get("/hodl/rates.json")]
        Task<List<CurrencyEntity>> GetRates();

        [Get("/hodl.staging/rates.json")]
        Task<List<CurrencyEntity>> GetStagingRates();

        [Get("/hodl/fee-estimator.json")]
        Task<FeeEntity> GetFeeEstimator();

        [Get("/precio.json?period={period}")]
        Task<PricesEntity> GetPrecioByPeriod([AliasAs("period")] string period);
    }
}
