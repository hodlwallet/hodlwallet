using System;
using System.Text;

using Security;

using Xamarin.Forms;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.iOS.Services;

[assembly: Dependency(typeof(iOSLegacySecureKeyService))]
namespace HodlWallet2.iOS.Services
{
    public class iOSLegacySecureKeyService : ILegacySecureKeyService
    {
        const string WAllET_SEC_ATTR_SERVICE = "co.hodlwallet";
        
        static T _LegacyGetKeychainItem<T>(string key)
        {
            byte[] data = null;

            using (var record = ExistingRecordForKey(key))
            using (var match = SecKeyChain.QueryAsRecord(record, out var resultCode))
            {
                if (resultCode == SecStatusCode.Success)
                {
                    data = match.ValueData.ToArray();
                }
            }

            if (data == null)
            {
                throw new InvalidOperationException(string.Format("GetKeychainItem: data was null for key: {0}", key));
            }

            var generic = typeof(T);

            if (generic == typeof(byte[]))
            {
                return (T)Convert.ChangeType(data, typeof(T));
            }
            if (generic == typeof(string))
            {
                return (T)Convert.ChangeType(Encoding.UTF8.GetString(data), typeof(T));
            }
            if (generic == typeof(long))
            {
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(data);

                return (T)Convert.ChangeType(BitConverter.ToInt64(data), typeof(T));
            }

            throw new ArgumentException("GetKeychainItem: Invalid type '{0}' was passed", typeof(T).ToString());
        }

        static SecRecord ExistingRecordForKey(string key)
        {
            return new SecRecord(SecKind.GenericPassword)
            {
                Account = key,
                Service = WAllET_SEC_ATTR_SERVICE
            };
        }

        public string GetMnemonic()
        {
            return _LegacyGetKeychainItem<string>(KeychainKey.MNEMONIC);
        }

        // Depricated
        public static byte[] GetSeed()
        {
            return _LegacyGetKeychainItem<byte[]>(KeychainKey.SEED);
        }

        public byte[] GetMasterPublicKey()
        {
            return _LegacyGetKeychainItem<byte[]>(KeychainKey.MASTER_PUB_KEY);
        }

        public string GetPin()
        {
            return _LegacyGetKeychainItem<string>(KeychainKey.PIN);
        }

        public long GetPinFailCount()
        {
            return _LegacyGetKeychainItem<long>(KeychainKey.PIN_FAIL_COUNT);
        }

        public long GetPinFailTime()
        {
            return _LegacyGetKeychainItem<long>(KeychainKey.PIN_FAIL_TIME);
        }

        public long GetWalletCreationTime()
        {
            return _LegacyGetKeychainItem<long>(KeychainKey.CREATION_TIME);
        }

        public long GetSpendLimit()
        {
            return _LegacyGetKeychainItem<long>(KeychainKey.SPEND_LIMIT);
        }

        public byte[] GetApiAuthKey()
        {
            return _LegacyGetKeychainItem<byte[]>(KeychainKey.API_AUTH_KEY);
        }

        struct KeychainKey
        {
            public const string MNEMONIC = "mnemonic";
            public const string CREATION_TIME = "creationtime";
            public const string MASTER_PUB_KEY = "masterpubkey";
            public const string SPEND_LIMIT = "spendlimit";
            public const string PIN = "pin";
            public const string PIN_FAIL_COUNT = "pinfailcount";
            public const string PIN_FAIL_TIME = "pinfailheight";
            public const string API_AUTH_KEY = "authprivkey";
            public const string SEED = "seed"; // deprecated
        }
    }
}
