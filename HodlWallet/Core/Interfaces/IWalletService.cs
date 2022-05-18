//
// IPrecioService.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 HODL Wallet
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using NBitcoin;

using Liviano.Models;
using Liviano.Interfaces;

namespace HodlWallet.Core.Interfaces
{
    public interface IWalletService
    {
        bool IsStarted { get; }
        bool IsConfigured { get; }

        bool Syncing { get; }

        IWallet Wallet { get; }

        Serilog.ILogger Logger { set; get; }

        event EventHandler OnConfigured;
        event EventHandler OnStarted;

        event EventHandler<DateTimeOffset> OnSyncStarted;
        event EventHandler<DateTimeOffset> OnSyncFinished;

        void Start();
        void InitializeWallet(string accountType = "standard");
        void StartWalletWithWalletId(string accountType = "standard");
        void DestroyWallet(bool dryRun = false);
        bool IsAddressOwn(string address);
        Money GetCurrentAccountBalanceInBTC(bool includeUnconfirmed);
        long GetCurrentAccountBalanceInSatoshis(bool includeUnconfirmed);
        BitcoinAddress GetReceiveAddress();
        Network GetNetwork();
        IEnumerable<Tx> GetCurrentAccountTransactions();
        (bool Success, Transaction Tx, decimal Fees, string Error) CreateTransaction(
            decimal amount, string addressTo,
            decimal feeSatsPerByte, string password
        );

        (bool Success, string Error) AddAccount(string type, string name, string color);
        string GetWordListLanguage();
        string GetColorByAccount(string accountId);
        Task<(bool Sent, string Error)> SendTransaction(Transaction tx);
    }
}
