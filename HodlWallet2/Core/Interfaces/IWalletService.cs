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

        IBroadcastManager BroadcastManager { get; set; }
        ITransactionManager TransactionManager { get; set; }
        IAsyncLoopFactory AsyncLoopFactory { get; set; }
        IDateTimeProvider DateTimeProvider { get; set; }
        IScriptAddressReader ScriptAddressReader { get; set; }
        IStorageProvider StorageProvider { get; set; }
        IWalletSyncManager WalletSyncManager { get; set; }
        WalletManager WalletManager { get; set; }
        NodesGroup NodesGroup { get; set; }
        BlockLocator ScanLocation { get; set; }
        HdAccount CurrentAccount { get; set; }

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
