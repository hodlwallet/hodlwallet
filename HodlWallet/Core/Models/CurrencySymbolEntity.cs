using System;
using System.Collections.Generic;
using System.Text;

namespace HodlWallet.Core.Models
{
    public class CurrencySymbolEntity
    {
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public float Rate { get; set; }
    }
}