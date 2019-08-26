﻿using System;
using System.Text;

using Security;

namespace HodlWalstring2.iOS.Services
{
    public class iOSLegacySecureKeyService
    {
        const string WAllET_SEC_ATTR_SERVICE = "co.hodlwallet";
        
        static T _LegacyGetKeychainItem<T>(string key)
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = WAllET_SEC_ATTR_SERVICE,
                Account = key
            };

            var data = SecKeyChain.QueryAsData(record).ToArray();

            if (data == null)
            {
                throw new InvalidOperationException(string.Format("GetKeychainItem: data was null: {0}", data));
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

        public static string GetMnemonic()
        {
            return _LegacyGetKeychainItem<string>(KeychainKey.MNEMONIC);
        }

        public static byte[] GetSeed()
        {
            return _LegacyGetKeychainItem<byte[]>(KeychainKey.SEED);
        }

        public static byte[] GetMasterPubKey()
        {
            return _LegacyGetKeychainItem<byte[]>(KeychainKey.MASTER_PUB_KEY);
        }

        public static string GetPin()
        {
            return _LegacyGetKeychainItem<string>(KeychainKey.PIN);
        }

        public static long GetPinFailCount()
        {
            return _LegacyGetKeychainItem<long>(KeychainKey.PIN_FAIL_COUNT);
        }

        public static long GetPinFailTime()
        {
            return _LegacyGetKeychainItem<long>(KeychainKey.PIN_FAIL_TIME);
        }

        public static byte[] GetCreationTime()
        {
            return _LegacyGetKeychainItem<byte[]>(KeychainKey.CREATION_TIME);
        }

        public static long GetSpendLimit()
        {
            return _LegacyGetKeychainItem<long>(KeychainKey.SPEND_LIMIT);
        }

        public static byte[] GetApiAuthkey()
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
            public const string USER_ACCOUNT = "https://api.breadwalstring.com";
            public const string SEED = "seed"; // deprecated
        }
    }
}
