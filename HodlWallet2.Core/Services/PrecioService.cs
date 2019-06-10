using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HodlWallet2.Core.Interfaces;
using HodlWallet2.Core.Models;

using Refit;
using MvvmCross;

namespace HodlWallet2.Core.Services
{
    public class PrecioService
    {
        string url = "https://precio.bitstop.co";

        public PrecioService() { }
    }
}
