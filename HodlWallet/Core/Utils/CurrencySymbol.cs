using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HodlWallet.Core.Utils
{
    public static class CurrencySymbol
    {
        public static List<CurrencySymbolEntity> CurrencySymbols { get; set; } = new();
        static List<CurrencyEntity> currencyEntitiesEndpoint = new();

        static IPrecioHttpService PrecioHttpService => CustomRefitSettings.RestClient();

        //public async Task CurrencySymbol()
        //{
        //    await PopulateCurrency();
        //}

        public static async Task<List<CurrencySymbolEntity>> PopulateCurrency()
        {
            //MimicEndpoint EndpointObj = new();
            currencyEntitiesEndpoint = await PrecioHttpService.GetRates();

            // Rate

            foreach (var currencyEntity in currencyEntitiesEndpoint)
            {
                CurrencySymbols.Add(new CurrencySymbolEntity
                {
                    Code = currencyEntity.Code,
                    Symbol = GetSymbol(currencyEntity.Code),
                    Name = currencyEntity.Name,
                    Rate = currencyEntity.Rate
                });
            }

            return CurrencySymbols;


            /*//currencyEntitiesEndpoint.ForEach(currencyEntity => {
            //    CurrencySymbols.Add(new CurrencySymbolEntity
            //    {
            //        Code = currencyEntity.Code,
            //        Symbol = GetSymbol(currencyEntity.Code),
            //        Name = currencyEntity.Name,
            //        Rate = currencyEntity.Rate
            //    });
            //});*/
        }

        public static string GetSymbol(string code)
        {
            CurrencySymbolEntity currency = Constants.CURRENCY_SYMBOLS.Find(c => c.Code == code);
            if (currency == null)
                return "";

            return currency.Symbol;
        }
    }
}