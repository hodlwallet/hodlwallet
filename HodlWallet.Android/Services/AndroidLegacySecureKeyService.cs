using System;
using System.Collections.Generic;
using System.Text;

using Android.Content;
using Android.Security.Keystore;
using Android.Util;

using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;

using Xamarin.Forms;

using HodlWallet.Core.Interfaces;
using HodlWallet.Droid.Services;

[assembly: Dependency(typeof(AndroidLegacySecureKeyService))]
namespace HodlWallet.Droid.Services
{
    public class AndroidLegacySecureKeyService : ILegacySecureKeyService
    {
        static readonly object locker = new object();

        internal static Context AppContext => Android.App.Application.Context;

        static byte[] _LegacyGetAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var context = AppContext;

            var obj = BRKeyStoreAliases.AliasObjectMap[key];

            if (obj == null) throw new KeyNotFoundException(string.Format("Key does not return an Alias Object: {0}", key));

            lock (locker)
            {
                var ks = KeyStore.GetInstance(BRKeyStoreAliases.ANDROID_KEY_STORE);
                ks.Load(null);
                ISecretKey secret = (ISecretKey)ks.GetKey(obj.Alias, null);

                var encryptedData = _RetrieveEncryptedData(context, obj.Alias);
                if (encryptedData != null)
                {
                    var iv = _RetrieveEncryptedData(context, obj.IVFileName);
                    if (iv == null)
                    {
                        throw new ArgumentNullException(string.Format("iv is missing when data is not: {0}", obj.Alias));
                    }

                    Cipher outCipher = Cipher.GetInstance(BRKeyStoreAliases.NEW_CIPHER_ALGORITHM);
                    outCipher.Init(CipherMode.DecryptMode, secret, new GCMParameterSpec(128, iv));

                    try
                    {
                        byte[] decryptedData = outCipher.DoFinal(encryptedData);
                        if (decryptedData != null)
                        {
                            return decryptedData;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new AndroidRuntimeException(string.Format("failed to decrypt data: {0}", ex.Message));
                    }
                }
                throw new KeyNotFoundException(string.Format("Alias did not return any data: {0}", obj.Alias));
            }
        }

        static byte[] _RetrieveEncryptedData(Context context, string name)
        {
            ISharedPreferences pref = context.GetSharedPreferences(BRKeyStoreAliases.KEY_STORE_PREFS_NAME, FileCreationMode.Private);
            string base64 = pref.GetString(name, null);
            if (base64 == null) return null;
            return Base64.Decode(base64, Base64Flags.Default);
        }

        static int _GetIntFromBytes(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt32(data);
        }

        static long _GetLongFromBytes(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt64(data);
        }

        public string GetMnemonic()
        {
            var data = _LegacyGetAsync(BRKeyStoreAliases.PHRASE_ALIAS);

            return Encoding.UTF8.GetString(data);
        }

        public byte[] GetMasterPublicKey()
        {
            return _LegacyGetAsync(BRKeyStoreAliases.PUB_KEY_ALIAS);
        }

        public byte[] GetApiAuthKey()
        {
            return _LegacyGetAsync(BRKeyStoreAliases.AUTH_KEY_ALIAS);
        }

        public long GetWalletCreationTime()
        {
            var data = _LegacyGetAsync(BRKeyStoreAliases.WALLET_CREATION_TIME_ALIAS);

            return _GetLongFromBytes(data);
        }

        public string GetPin()
        {
            var data = _LegacyGetAsync(BRKeyStoreAliases.PASS_CODE_ALIAS);
            var pinCode = Encoding.UTF8.GetString(data);

            try
            {
                int.Parse(pinCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("GetPinCode: WARNING passcode isn't a number: {0}\n{1}", pinCode, ex.Message));
                return "";
            }

            if (pinCode.Length != 6 && pinCode.Length != 4)
            {
                Console.WriteLine(string.Format("GetPinCode: WARNING passcode has invalid length: {0}", pinCode));
                return "";
            }

            return pinCode;
        }

        public long GetPinFailCount()
        {
            var data = _LegacyGetAsync(BRKeyStoreAliases.FAIL_COUNT_ALIAS);

            return _GetLongFromBytes(data);
        }

        public long GetSpendLimit()
        {
            var data = _LegacyGetAsync(BRKeyStoreAliases.SPEND_LIMIT_ALIAS);

            return _GetLongFromBytes(data);
        }

        public long GetPinFailTime()
        {
            var data = _LegacyGetAsync(BRKeyStoreAliases.FAIL_TIMESTAMP_ALIAS);

            return _GetLongFromBytes(data);
        }

        public static long GetTotalLimit()
        {
            var data = _LegacyGetAsync(BRKeyStoreAliases.TOTAL_LIMIT_ALIAS);

            return _GetLongFromBytes(data);
        }

        public static long GetLastPinUsedTime()
        {
            var data = _LegacyGetAsync(BRKeyStoreAliases.PASS_TIME_ALIAS);

            return _GetLongFromBytes(data);
        }

        public static byte[] GetToken()
        {
            return _LegacyGetAsync(BRKeyStoreAliases.TOKEN_ALIAS);
        }

        public static string GetCanary()
        {
            var data = _LegacyGetAsync(BRKeyStoreAliases.CANARY_ALIAS);

            return Encoding.UTF8.GetString(data);
        }
    }

    public static class BRKeyStoreAliases
    {
        public const string KEY_STORE_PREFS_NAME = "keyStorePrefs";

        public const string ANDROID_KEY_STORE = "AndroidKeyStore";

        public const string NEW_CIPHER_ALGORITHM = "AES/GCM/NoPadding";
        public const string NEW_PADDING = KeyProperties.EncryptionPaddingNone;
        public const string NEW_BLOCK_MODE = KeyProperties.BlockModeGcm;

        const string PHRASE_IV = "ivphrase";
        const string CANARY_IV = "ivcanary";
        const string PUB_KEY_IV = "ivpubkey";
        const string WALLET_CREATION_TIME_IV = "ivtime";
        const string PASS_CODE_IV = "ivpasscode";
        const string FAIL_COUNT_IV = "ivfailcount";
        const string SPEND_LIMIT_IV = "ivspendlimit";
        const string TOTAL_LIMIT_IV = "ivtotallimit";
        const string FAIL_TIMESTAMP_IV = "ivfailtimestamp";
        const string AUTH_KEY_IV = "ivauthkey";
        const string TOKEN_IV = "ivtoken";
        const string PASS_TIME_IV = "passtimetoken";

        public const string PHRASE_ALIAS = "phrase";
        public const string CANARY_ALIAS = "canary";
        public const string PUB_KEY_ALIAS = "pubKey";
        public const string WALLET_CREATION_TIME_ALIAS = "creationTime";
        public const string PASS_CODE_ALIAS = "passCode";
        public const string FAIL_COUNT_ALIAS = "failCount";
        public const string SPEND_LIMIT_ALIAS = "spendlimit";
        public const string TOTAL_LIMIT_ALIAS = "totallimit";
        public const string FAIL_TIMESTAMP_ALIAS = "failTimeStamp";
        public const string AUTH_KEY_ALIAS = "authKey";
        public const string TOKEN_ALIAS = "token";
        public const string PASS_TIME_ALIAS = "passTime";

        public const int AUTH_DURATION_SEC = 300;

        public static IDictionary<string, AliasObject> AliasObjectMap =
            new Dictionary<string, AliasObject>
            {
                { PHRASE_ALIAS, new AliasObject(PHRASE_ALIAS, PHRASE_IV) },
                { CANARY_ALIAS, new AliasObject(CANARY_ALIAS, CANARY_IV) },
                { PUB_KEY_ALIAS, new AliasObject(PUB_KEY_ALIAS, PUB_KEY_IV) },
                { WALLET_CREATION_TIME_ALIAS, new AliasObject(WALLET_CREATION_TIME_ALIAS, WALLET_CREATION_TIME_IV) },
                { PASS_CODE_ALIAS, new AliasObject(PASS_CODE_ALIAS, PASS_CODE_IV) },
                { FAIL_COUNT_ALIAS, new AliasObject(FAIL_COUNT_ALIAS, FAIL_COUNT_IV) },
                { SPEND_LIMIT_ALIAS, new AliasObject(SPEND_LIMIT_ALIAS, SPEND_LIMIT_IV) },
                { FAIL_TIMESTAMP_ALIAS, new AliasObject(FAIL_TIMESTAMP_ALIAS, FAIL_TIMESTAMP_IV) },
                { AUTH_KEY_ALIAS, new AliasObject(AUTH_KEY_ALIAS, AUTH_KEY_IV) },
                { TOKEN_ALIAS, new AliasObject(TOKEN_ALIAS, TOKEN_IV) },
                { PASS_TIME_ALIAS, new AliasObject(PASS_TIME_ALIAS, PASS_TIME_IV) },
                { TOTAL_LIMIT_ALIAS, new AliasObject(TOTAL_LIMIT_ALIAS, TOTAL_LIMIT_IV) }
            };
    }

    public class AliasObject
    {
        public string Alias;
        public string IVFileName;

        public AliasObject(string alias, string ivFileName)
        {
            Alias = alias;
            IVFileName = ivFileName;
        }
    }
}
