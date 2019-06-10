using System;
using Newtonsoft.Json;

namespace HodlWallet2.Core.Models
{
    public class FeeEntity
    {
        [JsonProperty("fastest_sat_per_kilobyte")]
        public long FastestSatKB { get; set; }

        [JsonProperty("fastest_time_text")]
        public string FastestTime { get; set; }

        [JsonProperty("fastest_blocks")]
        public int FastestBlocks { get; set; }

        [JsonProperty("normal_sat_per_kilobyte")]
        public long NormalSatKB { get; set; }

        [JsonProperty("normal_time_text")]
        public string NormalTime { get; set; }

        [JsonProperty("normal_blocks")]
        public int NormalBlocks { get; set; }

        [JsonProperty("slow_sat_per_kilobyte")]
        public long SlowSatKB { get; set; }

        [JsonProperty("slow_time_text")]
        public string SlowTime { get; set; }

        [JsonProperty("slow_blocks")]
        public int SlowBlocks { get; set; }
    }
}
