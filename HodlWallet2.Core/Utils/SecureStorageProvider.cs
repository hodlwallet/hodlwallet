using System.Linq;

using Xamarin.Essentials;

using NBitcoin;

using Liviano.Exceptions;
using Liviano.Utilities;
using Liviano;

namespace HodlWallet2.Core.Utils
{
    public class SecureStorageProvider
    {
        const string WALLET_ID_KEY = "wallet-id";
        const string PIN_KEY = "pin";
        const string MNEMONIC_KEY = "mnemonic";
        const string NETWORK_KEY = "network";
        const string SEED_BIRTHDAY = "seed-birthday";

        public static string GetWalletId()
        {
            return Get(WALLET_ID_KEY);
        }

        public static bool HasWalletId()
        {
            return !string.IsNullOrEmpty(GetWalletId());
        }

        public static void SetWalletId(string walletId)
        {
            Set(WALLET_ID_KEY, walletId);
        }

        public static string GetMnemonic()
        {
            return Get(MNEMONIC_KEY);
        }

        public static bool HasMnemonic()
        {
            return !string.IsNullOrEmpty(GetMnemonic());
        }

        public static void SetMnemonic(string mnemonic)
        {
            Guard.NotEmpty(mnemonic, nameof(mnemonic));

            if (!HdOperations.IsValidChecksum(mnemonic))
                throw new WalletException("Invalid mnemonic the checksum wasn't validated");

            Set(MNEMONIC_KEY, mnemonic);
        }

        public static string GetPin()
        {
            return Get(PIN_KEY);
        }

        public static bool HasPin()
        {
            return !string.IsNullOrEmpty(GetPin());
        }

        public static void SetPin(string pin)
        {
            Guard.NotEmpty(pin, nameof(pin));

            if (!pin.All(char.IsDigit))
                throw new WalletException($"Unable to set pin: '{pin}' includes charactes that aren't digits");

            if (pin.Length != 6)
                throw new WalletException($"Unable to set pin: '{pin}' it should be 6 digits long");

            SecureStorage.SetAsync(PIN_KEY, pin);
        }

        public static string GetNetwork()
        {
            return Get(NETWORK_KEY);
        }

        public static bool HasNetwork()
        {
            return !string.IsNullOrEmpty(GetNetwork());
        }

        public static void SetNetwork(string network)
        {
            Set(NETWORK_KEY, network);
        }

        public static void RemoveAll()
        {
            SecureStorage.RemoveAll();
        }

        private static string Get(string key)
        {
            return SecureStorage.GetAsync(key).Result;
        }
        
        private static void Set(string key, string val)
        {
            SecureStorage.SetAsync(key, val);
        }
    }
}