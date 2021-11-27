using HodlWallet.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HodlWallet.Core.Utils
{
    public class MimicEndpoint
    {
        public List<CurrencyEntity> Currency { get; set; } = new();
        public MimicEndpoint()
        {
            Currency.Add(new CurrencyEntity { Name = "US Dollar", Code = "USD", Rate = 54469.82f });
            Currency.Add(new CurrencyEntity { Name = "Australian Dollar", Code = "AUD", Rate = 1.55f });
            Currency.Add(new CurrencyEntity { Name = "Albanian Lek", Code = "ALL", Rate = 1.25f });
            Currency.Add(new CurrencyEntity { Name = "Algerian Dinar", Code = "DZD", Rate = 1.2f });
            Currency.Add(new CurrencyEntity { Name = "Argentine Peso", Code = "ARS", Rate = 3.05f });
            Currency.Add(new CurrencyEntity { Name = "Afghan Afghani", Code = "AFN", Rate = 4.05f });
            Currency.Add(new CurrencyEntity { Name = "Angolan Kwanza", Code = "AOA", Rate = 8.05f });
            Currency.Add(new CurrencyEntity { Name = "Armenian Dram", Code = "AMD", Rate = 2.05f });
            Currency.Add(new CurrencyEntity { Name = "Azerbaijani Manat", Code = "AZN", Rate = 6.03f });
        }
    }
}