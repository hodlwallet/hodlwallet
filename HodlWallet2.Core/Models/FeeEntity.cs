using System;
namespace HodlWallet2.Core.Models
{
    public class FeeEntity
    {
        public long FastestSatKB;
        public long NormalSatKB;
        public long SlowSatKB;

        public string FastestTime;
        public string NormalTime;
        public string SlowTime;

        public int FastestBlocks;
        public int NormalBlocks;
        public int SlowBlocks;

        public FeeEntity()
        {
        }
    }
}
