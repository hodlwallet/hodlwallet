//
// IPrecioService.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 
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
using NBitcoin.Protocol;

using Liviano.Models;
using Liviano.Utilities;
using Liviano.Interfaces;
using Liviano.Managers;

namespace HodlWallet2.Core.Interfaces
{
    public interface IWalletService
    {
        bool IsStarted { get; }
        bool IsConfigured { get; }

        Serilog.ILogger Logger { set; get; }

        NodesGroup NodesGroup { get; set; }
        BlockLocator ScanLocation { get; set; }
        IAccount CurrentAccount { get; set; }

        event EventHandler OnConfigured;
        event EventHandler OnStarted;
        event EventHandler OnScanning;
        event EventHandler OnScanningFinished;
        event EventHandler<int> OnConnectedNode;

        void InitializeWallet();
        void Configure(string walletId, string network, int? nodesToConnect);
        void Start(string password, DateTimeOffset? timeToStartOn);
        void StartWalletWithWalletId();
        void Scan(DateTimeOffset? timeToStartOn);
        void ReScan(DateTimeOffset? timeToStartOn);
        void DestroyWallet(bool dryRun = false);

        int GetLastSyncedBlockHeight();

        bool WalletExists();
        bool IsAddressOwn(string address);
        bool IsVerifyChecksum(string mnemonic, string wordList);
        bool IsWordInWordlist(string word, string wordList);
        bool IsSyncedToTip();
        string NewMnemonic(string wordList, int wordCount);
        string GetAddressFromTransaction(TransactionData txData);
        string GetLastSyncedDate();
        string GetSyncedProgressPercentage();
        string[] GenerateGuessWords(string wordToGuess, string language, int amountAround);

        decimal GetCurrentAccountBalanceInBTC(bool includeUnconfirmed);

        double GetSyncedProgress();

        long GetCurrentAccountBalanceInSatoshis(bool includeUnconfirmed);

        HdAddress GetReceiveAddress();
        Network GetNetwork();
        IEnumerable<TransactionData> GetCurrentAccountTransactions();
        (bool Success, Transaction Tx, decimal Fees, string Error) CreateTransaction(decimal amount, string addressTo,
            int feeSatsPerByte, string password);
        Task<(bool Sent, string Error)> SendTransaction(Transaction tx);
        ChainedBlock GetChainTip();
    }
}
