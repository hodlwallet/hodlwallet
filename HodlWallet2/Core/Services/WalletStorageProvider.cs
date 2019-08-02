using System;

using Liviano;

namespace HodlWallet2.Core
{
    public class WalletStorageProvider : FileSystemStorageProvider
    {
        public WalletStorageProvider(string id = null) : base(id, Environment.GetFolderPath(Environment.SpecialFolder.Personal))
        {
        }
    }
}
