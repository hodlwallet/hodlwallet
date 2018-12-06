using System;
using Liviano.Managers;
using Serilog;

namespace HodlWallet2
{
    public sealed class Wallet
    {
        private static Wallet instance = null;

        private static readonly object padlock = new object();

        private object _Lock = new object();

        public ILogger Logger { set; get; }

        Wallet()
        {
        }

        public static Wallet Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Wallet();
                    }
                    return instance;
                }
            }
        }

        public string NewMnemonic(string wordList = "english", int wordCount = 12)
        {
            return WalletManager.NewMnemonic(wordList, wordCount).ToString();
        }
    }

}
