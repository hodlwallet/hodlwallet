using System;

using Xamarin.Forms;

using Liviano.Interfaces;

namespace HodlWallet2.Core.Models
{
    public class AccountModel
    {
        public IAccount Account { get; private set; }

        public Color GradientStart { get; }

        public Color GradientEnd { get; }

        public AccountModel(IAccount account)
        {
            Account = account;
        }
    }
}
