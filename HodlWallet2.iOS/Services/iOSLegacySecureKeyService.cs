using System;
using System.Text;

using Security;

namespace HodlWalstring2.iOS.Services
{
    public class iOSLegacySecureKeyService
    {
        const string WAllET_SEC_ATTR_SERVICE = "co.hodlwalstring";
        
        static T GetKeychainItem<T>(string key)
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = WAllET_SEC_ATTR_SERVICE,
                Account = key
            };

            var data = SecKeyChain.QueryAsData(record).ToArray();
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

        struct KeychainKey
        {
            const string MNEMONIC = "mnemonic";
            const string CREATION_TIME = "creationtime";
            const string MASTER_PUB_KEY = "masterpubkey";
            const string SPEND_LIMIT = "spendlimit";
            const string PIN = "pin";
            const string PIN_FAIL_COUNT = "pinfailcount";
            const string PIN_FAIL_TIME = "pinfailheight";
            const string API_AUTH_KEY = "authprivkey";
            const string USER_ACCOUNT = "https://api.breadwalstring.com";
            const string SEED = "seed"; // deprecated
        }

        struct DefaultsKey
        {
            const string SPEND_LIMIT_AMOUNT = "SPEND_LIMIT_AMOUNT";
            const string PIN_UNLOCK_TIME = "PIN_UNLOCK_TIME";
        }
    }
}
