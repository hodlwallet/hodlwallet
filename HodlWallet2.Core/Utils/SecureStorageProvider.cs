using System;
using System.Linq;

using Xamarin.Essentials;

using Liviano;
using Liviano.Exceptions;
using Liviano.Utilities;

namespace HodlWallet2.Core.Utils
{
    public static class SecureStorageProvider
    {
        const string WALLET_ID_KEY = "wallet-id";
        const string PIN_KEY = "pin";
        const string PASSWORD_KEY = "password";
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
                throw new WalletException($"Unable to set pin: '{pin}' it must be 6 digits long");

            Set(PIN_KEY, pin);
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
            Guard.NotEmpty(password, nameof(password));

            Set(PASSWORD_KEY, password);
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

        public static int GetSeedBirthday()
        {
            var result = Get(SEED_BIRTHDAY);

            if (result == null) return -1;

            return int.Parse(Get(SEED_BIRTHDAY));
        }

        public static bool HasSeedBirthday()
        {
            return GetSeedBirthday() != -1;
        }

        public static void SetSeedBirthday(DateTimeOffset birthday)
        {
            Set(SEED_BIRTHDAY, birthday.ToUnixTimeSeconds().ToString());
        }

        public static void RemoveAll()
        {
            SecureStorage.RemoveAll();
        }

        static string Get(string key)
        {
            return SecureStorage.GetAsync(key).Result;
        }
        
        static void Set(string key, string val)
        {
            SecureStorage.SetAsync(key, val);
        }
    }
}