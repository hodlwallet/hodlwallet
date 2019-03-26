using System.Collections.Generic;
using HodlWallet2.Core.Models;

namespace HodlWallet2.Core.Interfaces
{
    public interface IFeatureSwitchingService <R, F>
        where R : struct
        where F : struct
    {
        R DefaultRelease { get; }

        R CurrentRelease { get; set; }

        bool IsDevMode { get; set; }

        R GetRelease(int releaseValue);

        FeatureSwitchingKeyPairValue<F>[] CurrentFeatures { get; }

        bool IsFeatureEnabled(F feature);

        void ChangeRelease(R release);

        string FriendlyReleaseName(R release);
        List<string> FriendlyReleases();
    }
}