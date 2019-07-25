using NBitcoin;

namespace HodlWallet2.Core.Extensions
{
    public static class NumbersExtensions
    {
        public static decimal Normalize(this Money value, MoneyUnit unit = MoneyUnit.BTC)
        {
            return value.ToDecimal(unit) / 1.000000000000000000000000000000000m;
        }

        public static decimal Normalize(this decimal value)
        {
            return value / 1.000000000000000000000000000000000m;
        }
    }
}
