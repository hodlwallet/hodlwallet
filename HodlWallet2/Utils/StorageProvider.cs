﻿using System;

using Liviano;

namespace HodlWallet2.Utils
{
    public class StorageProvider : FileSystemStorageProvider
    {
        public StorageProvider(string id = null) : base(id, Environment.GetFolderPath(Environment.SpecialFolder.Personal))
        {
        }
    }
}
