using System;
using System.Collections;
using System.Collections.Generic;

using Android.Security.Keystore;
using Android.Content;
using Android.Util;

using Java.Util.Concurrent.Locks;
using Java.IO;
using Java.Lang;
using Javax.Crypto;
using Java.Security;

namespace HodlWallet2.Droid.Services
{
    public class LegacySecureKeyService
    {
        public LegacySecureKeyService()
        {
        }
    }

    public class BRKeyStore
    {
        const string TAG = nameof(BRKeyStore);

        const string KEY_STORE_PREFS_NAME = "keyStorePrefs";

        public const string ANDROID_KEY_STORE = "AndroidKeyStore";

        public const string CIPHER_ALGORITHM = "AES/CBC/PKCS7Padding";
        public const string PADDING = KeyProperties.EncryptionPaddingPkcs7;
        public const string BLOCK_MODE = KeyProperties.BlockModeCbc;

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

        const string PHRASE_FILENAME = "my_phrase";
        const string CANARY_FILENAME = "my_canary";
        const string PUB_KEY_FILENAME = "my_pub_key";
        const string WALLET_CREATION_TIME_FILENAME = "my_creation_time";
        const string PASS_CODE_FILENAME = "my_pass_code";
        const string FAIL_COUNT_FILENAME = "my_fail_count";
        const string SPEND_LIMIT_FILENAME = "my_spend_limit";
        const string TOTAL_LIMIT_FILENAME = "my_total_limit";
        const string FAIL_TIMESTAMP_FILENAME = "my_fail_timestamp";
        const string AUTH_KEY_FILENAME = "my_auth_key";
        const string TOKEN_FILENAME = "my_token";
        const string PASS_TIME_FILENAME = "my_pass_time";
        bool _BugMessageShowing;

        public const int AUTH_DURATION_SEC = 300;
        static ReentrantLock _Lock = new ReentrantLock();

        public static IDictionary<string, AliasObject> AliasObjectMap =
            new Dictionary<string, AliasObject>
            {
                { PHRASE_ALIAS, new AliasObject(PHRASE_ALIAS, PHRASE_FILENAME, PHRASE_IV) },
                { CANARY_ALIAS, new AliasObject(CANARY_ALIAS, CANARY_FILENAME, CANARY_IV) },
                { PUB_KEY_ALIAS, new AliasObject(PUB_KEY_ALIAS, PUB_KEY_FILENAME, PUB_KEY_IV) },
                { WALLET_CREATION_TIME_ALIAS, new AliasObject(WALLET_CREATION_TIME_ALIAS, WALLET_CREATION_TIME_FILENAME, WALLET_CREATION_TIME_IV) },
                { PASS_CODE_ALIAS, new AliasObject(PASS_CODE_ALIAS, PASS_CODE_FILENAME, PASS_CODE_IV) },
                { FAIL_COUNT_ALIAS, new AliasObject(FAIL_COUNT_ALIAS, FAIL_COUNT_FILENAME, FAIL_COUNT_IV) },
                { SPEND_LIMIT_ALIAS, new AliasObject(SPEND_LIMIT_ALIAS, SPEND_LIMIT_FILENAME, SPEND_LIMIT_IV) },
                { FAIL_TIMESTAMP_ALIAS, new AliasObject(FAIL_TIMESTAMP_ALIAS, FAIL_TIMESTAMP_FILENAME, FAIL_TIMESTAMP_IV) },
                { AUTH_KEY_ALIAS, new AliasObject(AUTH_KEY_ALIAS, AUTH_KEY_FILENAME, AUTH_KEY_IV) },
                { TOKEN_ALIAS, new AliasObject(TOKEN_ALIAS, TOKEN_FILENAME, TOKEN_IV) },
                { PASS_TIME_ALIAS, new AliasObject(PASS_TIME_ALIAS, PASS_TIME_FILENAME, PASS_TIME_IV) },
                { TOTAL_LIMIT_ALIAS, new AliasObject(TOTAL_LIMIT_ALIAS, TOTAL_LIMIT_FILENAME, TOTAL_LIMIT_IV) }
            };

        static bool _setData(Context context, byte[] data, string alias, string aliasFile, string aliasiv,
                                int requestCode, bool authRequired)
        {
            _ValidateSet(data, alias, aliasFile, aliasiv, authRequired);

            KeyStore keyStore = null;

            try
            {
                lock (_Lock)
                {
                    keyStore = KeyStore.GetInstance(ANDROID_KEY_STORE);
                    keyStore.Load(null);

                    IKey secretKey = (ISecretKey)keyStore.GetKey(alias, null);
                    Cipher inCipher = Cipher.GetInstance(NEW_CIPHER_ALGORITHM);

                    if (secretKey == null)
                    {
                        secretKey = _CreateKeys(alias, authRequired);
                        inCipher.Init(CipherMode.EncryptMode, secretKey);
                    }
                    else
                    {
                        try
                        {
                            inCipher.Init(CipherMode.EncryptMode, secretKey);
                        }
                        catch (InvalidKeyException ignored)
                        {
                            if (ignored is UserNotAuthenticatedException)
                                throw ignored;

                            // Log(string.Format("_setData: OLD KEY PRESENT: {0}", alias));
                            secretKey = _CreateKeys(alias, authRequired);
                            inCipher.Init(CipherMode.EncryptMode, secretKey);
                        }
                    }

                    // The key cannot still be null
                    if (secretKey == null)
                    {
                        // Log(new KeyStoreException(string.Format("secret is null on _setData: {0}", alias)).toString());
                        return false;
                    }

                    byte[] iv = inCipher.GetIV();
                    if (iv == null)
                        throw new NullPointerException("iv is null!");

                    // Store the iv
                    _StoreEncryptedData(context, iv, aliasiv);
                    byte[] encryptedData = inCipher.DoFinal(data);
                    // Store the encrypted data
                    _StoreEncryptedData(context, encryptedData, alias);
                    return true;
                }
            }
            catch (UserNotAuthenticatedException e)
            {

            }
        }

        static void _ValidateSet(byte[] data, string alias, string aliasFile, string aliasiv, bool authRequired)
        {
            if (data == null)
            {
                throw new IllegalArgumentException("keystore insert data is null");
            }

            AliasObject obj = AliasObjectMap[alias];

            if (!obj.Alias.Equals(alias) || !obj.DataFileName.Equals(aliasFile) || !obj.IVFileName.Equals(aliasiv))
            {
                string err = string.Format("keystore insert inconsistency in names: {0}|{1}|{2}, obj: {3}|{4}|{5}", alias, aliasFile, aliasiv, obj.Alias, obj.DataFileName, obj.IVFileName);
                throw new IllegalArgumentException(err);
            }

            if (authRequired)
            {
                if (!alias.Equals(PHRASE_ALIAS) && !alias.Equals(CANARY_ALIAS))
                    throw new IllegalArgumentException(string.Format("keystore auth_required is true but alias is: {0}", alias));
            }
        }

        static ISecretKey _CreateKeys(string alias, bool authRequired)
        {
            KeyGenerator keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, ANDROID_KEY_STORE);

            // Set the alias of the entry in Android KeyStore where the key will appear
            // and the constrains (purposes) in the constructor of the Builder
            keyGenerator.Init(new KeyGenParameterSpec.Builder(alias,
                            KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                        .SetBlockModes(NEW_BLOCK_MODE)
                        .SetUserAuthenticationRequired(authRequired)
                        .SetUserAuthenticationValidityDurationSeconds(AUTH_DURATION_SEC)
                        .SetRandomizedEncryptionRequired(false)
                        .SetEncryptionPaddings(NEW_PADDING)
                        .Build());

            return keyGenerator.GenerateKey();
        }

        public static void _StoreEncryptedData(Context context, byte[] data, string name)
        {
            ISharedPreferences pref = context.GetSharedPreferences(KEY_STORE_PREFS_NAME, FileCreationMode.Private);
            string base64 = Base64.EncodeToString(data, Base64Flags.Default);
            ISharedPreferencesEditor edit = pref.Edit();
            edit.PutString(name, base64);
            edit.Apply();
        }
    }

    public class AliasObject
    {
        public string Alias;
        public string DataFileName;
        public string IVFileName;

        public AliasObject(string alias, string datafileName, string ivFileName)
        {
            Alias = alias;
            DataFileName = datafileName;
            IVFileName = ivFileName;
        }
    }
}
