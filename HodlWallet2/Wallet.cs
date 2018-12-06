using System;
using Liviano.Managers;
using Serilog;

namespace HodlWallet2
{
    public class Wallet
    {
        private object _Lock = new object();

        public ILogger Logger { set; get; }

        public Wallet()
        {
        }

        public string NewMnemonic(string wordList = "english", int wordCount = 12)
        {
            return WalletManager.NewMnemonic(wordList, wordCount).ToString();
        }
    }
}

