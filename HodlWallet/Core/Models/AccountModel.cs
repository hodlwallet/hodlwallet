//
// AccountModel.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2021 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms;

using Liviano.Interfaces;
using HodlWallet.Core.Utils;

namespace HodlWallet.Core.Models
{
    public class AccountModel
    {
        public IAccount AccountData { get; private set; }
        public string AccountName { get; private set; }
        public string Balance { get; private set; }
        public string AccountColorCode { get; set; }
        public static AccountModel FromAccountData(IAccount account)
        {
            return new AccountModel
            {
                AccountData = account,
                AccountName = account.Name,
                Balance = account.GetBalance().ToString(),
                AccountColorCode = Constants.DEFAULT_ACCOUNT_COLOR_CODE
            };
        }

        static List<IAccount> GetAccountListFromCollection (ObservableCollection<AccountModel> accountCollection)
        {
            List<IAccount> AccountList = new List<IAccount>();
        
            AccountList.AddRange(accountCollection.Select(item => item.AccountData).ToArray());

            return AccountList;
        }

        public static void SyncCollections(List<IAccount> mainAccountsList, ObservableCollection<AccountModel> menuAccounts)
        {
            // Compare and look for the missing items in the menu accounts.
            var accountsToSync = mainAccountsList.Except(GetAccountListFromCollection(menuAccounts));
            foreach (var item in accountsToSync)
            {
                menuAccounts.Add(FromAccountData(item));
            }
        }
    }
}
