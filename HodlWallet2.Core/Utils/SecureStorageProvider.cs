using Xamarin.Essentials;

namespace HodlWallet2.Core.Utils
{
    public class SecureStorageProvider
    {
        const string WALLET_ID_KEY = "wallet-id";
        const string PASSWORD_KEY = "password";
        const string MNEMONIC_KEY = "mnemonic";
        const string FINGERPRINT_KEY = "fingerprint"; // TODO Why where these 2 added? - Igor.
        const string MNEMONIC_STATUS_KEY = "mnemonic-status";
        const string NETWORK_KEY = "network";

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
            Set(MNEMONIC_KEY, mnemonic);
        }

        public static string GetPassword()
        {
            return Get(PASSWORD_KEY);
        }

        public static bool HasPassword()
        {
            return !string.IsNullOrEmpty(GetPassword());
        }

        public static void SetPassword(string password)
        {
            SecureStorage.SetAsync(PASSWORD_KEY, password);
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