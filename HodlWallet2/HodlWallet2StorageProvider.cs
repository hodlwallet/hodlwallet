using System;

using Liviano;

namespace HodlWallet2
{
    public class HodlWallet2StorageProvider : FileSystemStorageProvider
    {
        public HodlWallet2StorageProvider(string id = null) : base(id, Environment.GetFolderPath(Environment.SpecialFolder.Personal))
        {
        }
    }
}
