using HodlWallet.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HodlWallet.Core.Utils
{
    public class CurrencySymbol
    {
        public List<CurrencySymbolEntity> CurrencySymbols { get; set; } = new();
        List<CurrencyEntity> currencyEntitiesEndpoint = new();

        public CurrencySymbol()
        {
            PopulateCurrency();            
        }

        private void PopulateCurrency()
        {
            MimicEndpoint EndpointObj = new();
            currencyEntitiesEndpoint = EndpointObj.Currency;

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
