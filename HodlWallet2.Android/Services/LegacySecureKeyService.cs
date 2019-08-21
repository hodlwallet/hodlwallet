using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Security;
using Android.Security.Keystore;

using Java.Security;
using Java.Util.Concurrent.Locks;
using Javax.Crypto;
using Javax.Crypto.Spec;

using Xamarin.Essentials;

namespace HodlWallet2.Droid.Services
{
    class LegacySecureKeyUtils
    {
        internal static Context AppContext => Application.Context;

        internal static bool HasApiLevel(BuildVersionCodes versionCode) =>
            (int)Build.VERSION.SdkInt >= (int)versionCode;

        internal static bool HasApiLevelN =>
#if __ANDROID_24__
            HasApiLevel(BuildVersionCodes.N);
#else
            false;
#endif

        internal static string Md5Hash(string input)
        {
            var hash = new System.Text.StringBuilder();
            var md5provider = new MD5CryptoServiceProvider();
            var bytes = md5provider.ComputeHash(Encoding.UTF8.GetBytes(input));

            for (var i = 0; i < bytes.Length; i++)
                hash.Append(bytes[i].ToString("x2"));

            return hash.ToString();
        }

        internal static void SetLocale(Java.Util.Locale locale)
        {
            Java.Util.Locale.Default = locale;
            var resources = AppContext.Resources;
            var config = resources.Configuration;

            if (HasApiLevelN)
                config.SetLocale(locale);
            else
                config.Locale = locale;

#pragma warning disable CS0618 // Type or member is obsolete
            resources.UpdateConfiguration(config, resources.DisplayMetrics);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        internal static Java.Util.Locale GetLocale()
        {
            var resources = AppContext.Resources;
            var config = resources.Configuration;
#if __ANDROID_24__
            if (HasApiLevelN)
                return config.Locales.Get(0);
#endif

            return config.Locale;
        }
    }

    public static partial class LegacySecureKeyService
    {
        internal static readonly string Alias = $"{AppInfo.PackageName}.xamarinessentials";

        static readonly object locker = new object();

        static Task<string> PlatformGetAsync(string key)
        {
            var context = LegacySecureKeyUtils.AppContext;

            string defaultEncStr = null;
            var encStr = Preferences.Get(LegacySecureKeyUtils.Md5Hash(key), defaultEncStr, Alias);

            string decryptedData = null;
            if (!string.IsNullOrEmpty(encStr))
            {
                try
                {
                    var encData = Convert.FromBase64String(encStr);
                    lock (locker)
                    {
                        var ks = new AndroidKeyStore(context, Alias, AlwaysUseAsymmetricKeyStorage);
                        decryptedData = ks.Decrypt(encData);
                    }
                }
                catch (AEADBadTagException)
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to decrypt key, {key}, which is likely due to an app uninstall. Removing old key and returning null.");
                    PlatformRemove(key);
                }
            }

            return Task.FromResult(decryptedData);
        }

        static Task PlatformSetAsync(string key, string data)
        {
            var context = LegacySecureKeyUtils.AppContext;

            byte[] encryptedData = null;
            lock (locker)
            {
                var ks = new AndroidKeyStore(context, Alias, AlwaysUseAsymmetricKeyStorage);
                encryptedData = ks.Encrypt(data);
            }

            var encStr = Convert.ToBase64String(encryptedData);
            Preferences.Set(LegacySecureKeyUtils.Md5Hash(key), encStr, Alias);

            return Task.CompletedTask;
        }

        static bool PlatformRemove(string key)
        {
            var context = LegacySecureKeyUtils.AppContext;

            key = LegacySecureKeyUtils.Md5Hash(key);
            Preferences.Remove(key, Alias);

            return true;
        }

        static void PlatformRemoveAll() =>
            Preferences.Clear(Alias);

        internal static bool AlwaysUseAsymmetricKeyStorage { get; set; } = false;
    }

    class AndroidKeyStore
    {
        const string androidKeyStore = "AndroidKeyStore"; // this is an Android const value
        const string aesAlgorithm = "AES";
        const string cipherTransformationAsymmetric = "RSA/ECB/PKCS1Padding";
        const string cipherTransformationSymmetric = "AES/GCM/NoPadding";
        const string prefsMasterKey = "SecureStorageKey";
        const int initializationVectorLen = 12; // Android supports an IV of 12 for AES/GCM

        internal AndroidKeyStore(Context context, string keystoreAlias, bool alwaysUseAsymmetricKeyStorage)
        {
            alwaysUseAsymmetricKey = alwaysUseAsymmetricKeyStorage;
            appContext = context;
            alias = keystoreAlias;

            keyStore = KeyStore.GetInstance(androidKeyStore);
            keyStore.Load(null);
        }

        readonly Context appContext;
        readonly string alias;
        readonly bool alwaysUseAsymmetricKey;
        readonly string useSymmetricPreferenceKey = "essentials_use_symmetric";

        KeyStore keyStore;
        bool useSymmetric = false;

        ISecretKey GetKey()
        {
            // check to see if we need to get our key from past-versions or newer versions.
            // we want to use symmetric if we are >= 23 or we didn't set it previously.

            useSymmetric = Preferences.Get(useSymmetricPreferenceKey, LegacySecureKeyUtils.HasApiLevel(BuildVersionCodes.M), LegacySecureKeyService.Alias);

            // If >= API 23 we can use the KeyStore's symmetric key
            if (useSymmetric && !alwaysUseAsymmetricKey)
                return GetSymmetricKey();

            // NOTE: KeyStore in < API 23 can only store asymmetric keys
            // specifically, only RSA/ECB/PKCS1Padding
            // So we will wrap our symmetric AES key we just generated
            // with this and save the encrypted/wrapped key out to
            // preferences for future use.
            // ECB should be fine in this case as the AES key should be
            // contained in one block.

            // Get the asymmetric key pair
            var keyPair = GetAsymmetricKeyPair();

            var existingKeyStr = Preferences.Get(prefsMasterKey, null, alias);

            if (!string.IsNullOrEmpty(existingKeyStr))
            {
                try
                {
                    var wrappedKey = Convert.FromBase64String(existingKeyStr);

                    var unwrappedKey = UnwrapKey(wrappedKey, keyPair.Private);
                    var kp = unwrappedKey.JavaCast<ISecretKey>();

                    return kp;
                }
                catch (InvalidKeyException ikEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to unwrap key: Invalid Key. This may be caused by system backup or upgrades. All secure storage items will now be removed. {ikEx.Message}");
                }
                catch (IllegalBlockSizeException ibsEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to unwrap key: Illegal Block Size. This may be caused by system backup or upgrades. All secure storage items will now be removed. {ibsEx.Message}");
                }
                catch (BadPaddingException paddingEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to unwrap key: Bad Padding. This may be caused by system backup or upgrades. All secure storage items will now be removed. {paddingEx.Message}");
                }
                SecureStorage.RemoveAll();
            }

            var keyGenerator = KeyGenerator.GetInstance(aesAlgorithm);
            var defSymmetricKey = keyGenerator.GenerateKey();

            var newWrappedKey = WrapKey(defSymmetricKey, keyPair.Public);

            Preferences.Set(prefsMasterKey, Convert.ToBase64String(newWrappedKey), alias);

            return defSymmetricKey;
        }

        // API 23+ Only
        ISecretKey GetSymmetricKey()
        {
            Preferences.Set(useSymmetricPreferenceKey, true, LegacySecureKeyService.Alias);

            var existingKey = keyStore.GetKey(alias, null);

            if (existingKey != null)
            {
                var existingSecretKey = existingKey.JavaCast<ISecretKey>();
                return existingSecretKey;
            }

            var keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, androidKeyStore);
            var builder = new KeyGenParameterSpec.Builder(alias, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                .SetBlockModes(KeyProperties.BlockModeGcm)
                .SetEncryptionPaddings(KeyProperties.EncryptionPaddingNone)
                .SetRandomizedEncryptionRequired(false);

            keyGenerator.Init(builder.Build());

            return keyGenerator.GenerateKey();
        }

        KeyPair GetAsymmetricKeyPair()
        {
            // set that we generated keys on pre-m device.
            Preferences.Set(useSymmetricPreferenceKey, false, LegacySecureKeyService.Alias);

            var asymmetricAlias = $"{alias}.asymmetric";

            var privateKey = keyStore.GetKey(asymmetricAlias, null)?.JavaCast<IPrivateKey>();
            var publicKey = keyStore.GetCertificate(asymmetricAlias)?.PublicKey;

            // Return the existing key if found
            if (privateKey != null && publicKey != null)
                return new KeyPair(publicKey, privateKey);

            var originalLocale = LegacySecureKeyUtils.GetLocale();
            try
            {
                // Force to english for known bug in date parsing:
                // https://issuetracker.google.com/issues/37095309
                LegacySecureKeyUtils.SetLocale(Java.Util.Locale.English);

                // Otherwise we create a new key
                var generator = KeyPairGenerator.GetInstance(KeyProperties.KeyAlgorithmRsa, androidKeyStore);

                var end = DateTime.UtcNow.AddYears(20);
                var startDate = new Java.Util.Date();
#pragma warning disable CS0618 // Type or member is obsolete
                var endDate = new Java.Util.Date(end.Year, end.Month, end.Day);
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618
                var builder = new KeyPairGeneratorSpec.Builder(LegacySecureKeyUtils.AppContext)
                    .SetAlias(asymmetricAlias)
                    .SetSerialNumber(Java.Math.BigInteger.One)
                    .SetSubject(new Javax.Security.Auth.X500.X500Principal($"CN={asymmetricAlias} CA Certificate"))
                    .SetStartDate(startDate)
                    .SetEndDate(endDate);

                generator.Initialize(builder.Build());
#pragma warning restore CS0618

                return generator.GenerateKeyPair();
            }
            finally
            {
                LegacySecureKeyUtils.SetLocale(originalLocale);
            }
        }

        byte[] WrapKey(IKey keyToWrap, IKey withKey)
        {
            var cipher = Cipher.GetInstance(cipherTransformationAsymmetric);
            cipher.Init(Javax.Crypto.CipherMode.WrapMode, withKey);
            return cipher.Wrap(keyToWrap);
        }

        IKey UnwrapKey(byte[] wrappedData, IKey withKey)
        {
            var cipher = Cipher.GetInstance(cipherTransformationAsymmetric);
            cipher.Init(Javax.Crypto.CipherMode.UnwrapMode, withKey);
            var unwrapped = cipher.Unwrap(wrappedData, KeyProperties.KeyAlgorithmAes, KeyType.SecretKey);
            return unwrapped;
        }

        internal byte[] Encrypt(string data)
        {
            var key = GetKey();

            // Generate initialization vector
            var iv = new byte[initializationVectorLen];

            var sr = new SecureRandom();
            sr.NextBytes(iv);

            Cipher cipher;

            // Attempt to use GCMParameterSpec by default
            try
            {
                cipher = Cipher.GetInstance(cipherTransformationSymmetric);
                cipher.Init(Javax.Crypto.CipherMode.EncryptMode, key, new GCMParameterSpec(128, iv));
            }
            catch (InvalidAlgorithmParameterException)
            {
                // If we encounter this error, it's likely an old bouncycastle provider version
                // is being used which does not recognize GCMParameterSpec, but should work
                // with IvParameterSpec, however we only do this as a last effort since other
                // implementations will error if you use IvParameterSpec when GCMParameterSpec
                // is recognized and expected.
                cipher = Cipher.GetInstance(cipherTransformationSymmetric);
                cipher.Init(Javax.Crypto.CipherMode.EncryptMode, key, new IvParameterSpec(iv));
            }

            var decryptedData = Encoding.UTF8.GetBytes(data);
            var encryptedBytes = cipher.DoFinal(decryptedData);

            // Combine the IV and the encrypted data into one array
            var r = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, r, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, r, iv.Length, encryptedBytes.Length);

            return r;
        }

        internal string Decrypt(byte[] data)
        {
            if (data.Length < initializationVectorLen)
                return null;

            var key = GetKey();

            // IV will be the first 16 bytes of the encrypted data
            var iv = new byte[initializationVectorLen];
            Buffer.BlockCopy(data, 0, iv, 0, initializationVectorLen);

            Cipher cipher;

            // Attempt to use GCMParameterSpec by default
            try
            {
                cipher = Cipher.GetInstance(cipherTransformationSymmetric);
                cipher.Init(Javax.Crypto.CipherMode.DecryptMode, key, new GCMParameterSpec(128, iv));
            }
            catch (InvalidAlgorithmParameterException)
            {
                // If we encounter this error, it's likely an old bouncycastle provider version
                // is being used which does not recognize GCMParameterSpec, but should work
                // with IvParameterSpec, however we only do this as a last effort since other
                // implementations will error if you use IvParameterSpec when GCMParameterSpec
                // is recognized and expected.
                cipher = Cipher.GetInstance(cipherTransformationSymmetric);
                cipher.Init(Javax.Crypto.CipherMode.DecryptMode, key, new IvParameterSpec(iv));
            }

            // Decrypt starting after the first 16 bytes from the IV
            var decryptedData = cipher.DoFinal(data, initializationVectorLen, data.Length - initializationVectorLen);

            return Encoding.UTF8.GetString(decryptedData);
        }
    }

    public class BRKeyStoreAliases
    {
        const string TAG = nameof(BRKeyStoreAliases);

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
