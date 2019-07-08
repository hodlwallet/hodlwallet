using System;

using ZXing;

namespace HodlWallet2.Core.Interactions
{
    public class BarcodeScannerPrompt
    {
        public Action<Result> ResultCallback { set; get; }
    }
}
