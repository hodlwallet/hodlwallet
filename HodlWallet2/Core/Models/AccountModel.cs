using System;

using Xamarin.Forms;

using Liviano.Interfaces;

namespace HodlWallet2.Core.Models
{
    public class AccountModel
    {
        public IAccount Account { get; private set; }

        public Color GradientStart { get; set; }

        public Color GradientEnd { get; set; }

        public static AccountModel FromAccountData(IAccount account)
        {

            return new AccountModel
            {
                Account = account,
                GradientStart = Color.FromHex(RandomHex()),
                GradientEnd = Color.FromHex(RandomHex())
            };
        }

        static string RandomHex()
        {
            // TODO Add option for light or dark preference.

            var rng = new Random();

            var rgb = (rng.Next(255), rng.Next(255), rng.Next(255));

            return $"{rgb.Item1:X2}{rgb.Item2:X2}{rgb.Item3:X2}";
        }
    }
}
