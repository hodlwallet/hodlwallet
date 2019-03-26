using System.Collections.Generic;
using System.Linq;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Models;
using Xamarin.Forms;

namespace HodlWallet2.Core.Services
{
    public enum Release : int
    {
        AllFeatures = 0,
        Release1 = 1,
    }

    public enum Feature
    {
        Onboarding,
    }

    public class FeatureSwitchingService : IFeatureSwitchingService<Release, Feature>
    {
        public bool IsDevMode { get; set; }

        private readonly Release DEFAULT_RELEASE_LEVEL = Release.Release1;

        private readonly FeatureSwitchingKeyPairValue<Feature>[] RELEASE1_FEATURES = new FeatureSwitchingKeyPairValue<Feature>[]
        {
            //iOS
            new FeatureSwitchingKeyPairValue<Feature>() { Feature = Feature.Onboarding, DevicePlatform = Device.iOS },
            //Android
            new FeatureSwitchingKeyPairValue<Feature>() { Feature = Feature.Onboarding, DevicePlatform = Device.Android },
        };

        private readonly FeatureSwitchingKeyPairValue<Feature>[] UNRELEASED_FEATURES = new FeatureSwitchingKeyPairValue<Feature>[]
        {
        };

        public Release DefaultRelease => DEFAULT_RELEASE_LEVEL;

        public Release CurrentRelease { get; set; }
        public FeatureSwitchingKeyPairValue<Feature>[] CurrentFeatures { get; private set; }

        public static FeatureSwitchingService Instance { get; } = new FeatureSwitchingService();

        private FeatureSwitchingService()
        {
            IsDevMode = false;
            ChangeRelease(DefaultRelease);
        }

        public Release GetRelease(int releaseValue)
        {
            return (Release)releaseValue;
        }

        public void ChangeRelease(Release release)
        {
            CurrentRelease = release;

            switch (release)
            {
                case Release.AllFeatures:
                    CurrentFeatures = GetAllFeatures();
                    break;
                case Release.Release1:
                    CurrentFeatures = RELEASE1_FEATURES;
                    break;
                default:
                    CurrentFeatures = GetAllFeatures();
                    break;
            }
        }

        public bool IsFeatureEnabled(Feature feature)
        {
            var teamConnectFeature = new FeatureSwitchingKeyPairValue<Feature> { DevicePlatform = Device.RuntimePlatform, Feature = feature };
            var results = false;
            switch (CurrentRelease)
            {
                case Release.AllFeatures: { results = true; break; }
                case Release.Release1:
                {
                    results = RELEASE1_FEATURES.Any(x => x.DevicePlatform == Device.RuntimePlatform && x.Feature == feature); 
                    break;
                }
            }
            return results;
        }

        public List<string> FriendlyReleases()
        {
            return new List<string>
            {
                "All Features",
                "Release 1",
            };
        }

        public string FriendlyReleaseName(Release release)
        {
            switch (release)
            {
                case Release.AllFeatures: return "All Features";
                case Release.Release1: return "Release 1";
                
                default: return release.ToString();
            }
        }

        private FeatureSwitchingKeyPairValue<Feature>[] GetAllFeatures()
        {
            var features = new List<FeatureSwitchingKeyPairValue<Feature>>();
            features.AddRange(RELEASE1_FEATURES);
            features.AddRange(UNRELEASED_FEATURES);
            return features.ToArray();
        }
    }
}