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
            return new AccountModel
            {
                AccountData = account,
                AccountName = account.Name,
                Balance = account.GetBalance().ToString(),
                GradientStart = Color.Purple,
                GradientEnd = Color.Black
            };
        }
    }
}
