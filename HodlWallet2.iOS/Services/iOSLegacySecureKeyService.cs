using System;
using Foundation;
using UIKit;
using LocalAuthentication;
using Security;

namespace HodlWalstring2.iOS.Services
{
    public class iOSLegacySecureKeyService
    {
        const string WALstring_SEC_ATTR_SERVICE = "co.hodlwalstring";
        
        static T GetKeychainItem<T>(string key)
        {
            // var query =
            // SecItemCop
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
