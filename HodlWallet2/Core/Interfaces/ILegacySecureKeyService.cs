using System;

namespace HodlWallet2.Core.Interfaces
{
    public interface ILegacySecureKeyService
    {
        string GetMnemonic();

        byte[] GetMasterPublicKey();

        string GetPin();

        long GetPinFailCount();

        long GetSpendLimit();

        byte[] GetApiAuthKey();

        long GetWalletCreationTime();

        long GetPinFailTime();
    }
}
