using Xamarin.Essentials;

namespace HodlWallet2.Core.Utils
{
    public class SecureStorageProvider
    {
        private const string WALLET_ID_KEY = "wallet-id";
        private const string PASSWORD_KEY = "password";
        private const string MNEMONIC_KEY = "mnemonic";
        private const string FINGERPRINT_KEY = "fingerprint";
        private const string MNEMONIC_STATUS_KEY = "mnemonic-status";

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