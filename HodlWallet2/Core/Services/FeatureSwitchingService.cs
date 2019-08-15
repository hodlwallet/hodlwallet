//
// FeatureSwitchingService.cs
//
// Copyright (c) 2019 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Collections.Generic;
using System.Linq;
using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Models;
using Xamarin.Forms;

namespace HodlWallet2.Core.Services
{
    public enum Release
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

        private readonly FeatureSwitchingKeyPairValue<Feature>[] RELEASE1_FEATURES = {
            //iOS
            new FeatureSwitchingKeyPairValue<Feature>() { Feature = Feature.Onboarding, DevicePlatform = Device.iOS },
            //Android
            new FeatureSwitchingKeyPairValue<Feature>() { Feature = Feature.Onboarding, DevicePlatform = Device.Android },
        };

        private readonly FeatureSwitchingKeyPairValue<Feature>[] UNRELEASED_FEATURES = {
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