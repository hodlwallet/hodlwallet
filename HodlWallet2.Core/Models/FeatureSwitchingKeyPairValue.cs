namespace HodlWallet2.Core.Models
{
    public class FeatureSwitchingKeyPairValue<T>
    {
        public T Feature { get; set; }
        public string DevicePlatform { get; set; }
    }
}