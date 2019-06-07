using System;
namespace HodlWallet2.Core.Models
{
    public class CurrencyEntity
    {
        public string code;
        public string name;
        public float rate;

        public CurrencyEntity(string code, string name, float rate)
        {
            this.code = code;
            this.name = name;
            this.rate = rate;
        }
    }
}
