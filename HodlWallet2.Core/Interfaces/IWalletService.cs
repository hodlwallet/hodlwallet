using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Liviano.Interfaces;
using Liviano.Managers;
using Liviano.Models;
using Liviano.Utilities;
using NBitcoin;
using NBitcoin.Protocol;
using Xamarin.Forms.Xaml;

namespace HodlWallet2.Core.Interfaces
{
    public interface IWalletService
    {
        Serilog.ILogger Logger { set; get; }
        bool IsStarted { get; }
        bool IsConfigured { get; }
        WalletManager WalletManager { get; set; }
        IBroadcastManager BroadcastManager { get; set; }
        ITransactionManager TransactionManager { get; set; }
        IAsyncLoopFactory AsyncLoopFactory { get; set; }
        IDateTimeProvider DateTimeProvider { get; set; }
        IScriptAddressReader ScriptAddressReader { get; set; }
        IStorageProvider StorageProvider { get; set; }
        IWalletSyncManager WalletSyncManager { get; set; }
        NodesGroup NodesGroup { get; set; }
        BlockLocator ScanLocation { get; set; }
        HdAccount CurrentAccount { get; set; }

        event EventHandler OnConfigured;
        event EventHandler OnStarted;
        event EventHandler OnScanning;
        
        void InitializeWallet();
        void Configure(string walletId, string network, int? nodesToConnect);
        void Start(string password, DateTimeOffset? timeToStartOn);
        void Scan(DateTimeOffset? timeToStartOn);
        void ReScan(DateTimeOffset? timeToStartOn);
        bool WalletExists();
        string NewMnemonic(string wordList, int wordCount);
        bool IsWordInWordlist(string word, string wordList);
        string[] GenerateGuessWords(string wordToGuess, string language, int amountAround);
        bool IsAddressReused(string address);
        bool IsVerifyChecksum(string mnemonic, string wordList);
        string GetAddressFromTransaction(TransactionData txData);
        HdAddress GetReceiveAddress();
        IEnumerable<TransactionData> GetCurrentAccountTransactions();
        (bool Success, Transaction Tx, decimal Fees, string Error) CreateTransaction(decimal amount, string addressTo,
            int feeSatsPerByte, string password);

        Task<(bool Sent, string Error)> SendTransaction(Transaction tx);
    }
}