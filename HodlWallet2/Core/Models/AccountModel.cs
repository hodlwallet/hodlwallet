using System;

using Liviano.Interfaces;

namespace HodlWallet2.Core.Models
{
    public class AccountModel
    {
        public IAccount Account { get; set; }

        public AccountModel(IAccount account)
        {
            Account = account;
        }
    }
}
