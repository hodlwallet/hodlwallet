using System;

using Xamarin.Forms;

using Liviano.Interfaces;

namespace HodlWallet2.Core.Models
{
    public class AccountModel
    {
        public IAccount AccountData { get; private set; }

        public string AccountName { get; private set; }

        public string Balance { get; private set; }

        public Color GradientStart { get; set; }

        public Color GradientEnd { get; set; }

        public static AccountModel FromAccountData(IAccount account)
        {
            var colors = RandomHex();

            return new AccountModel
            {
                AccountData = account,
                AccountName = account.Name,
                Balance = account.GetBalance().ToString(),
                GradientStart = Color.FromHex(colors.Item1),
                GradientEnd = Color.FromHex(colors.Item2)
            };
        }

        static (string, string) RandomHex()
        {
            // TODO Generate and store hex in wallet.
            //      Add option for light or dark preference.

            var rng = new Random();

            var startRGB = (rng.Next(128, 200), rng.Next(128, 200), rng.Next(128, 200));

            var endRGB = ((int)(startRGB.Item1 / 1.25), (int)(startRGB.Item2 / 1.25), (int)(startRGB.Item3 / 1.25));

            return ($"{startRGB.Item1:X2}{startRGB.Item2:X2}{startRGB.Item3:X2}",
                    $"{endRGB.Item1:X2}{endRGB.Item2:X2}{endRGB.Item3:X2}");
        }
    }
}
