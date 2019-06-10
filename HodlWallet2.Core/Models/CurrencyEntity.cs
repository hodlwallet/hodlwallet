using System;
using Newtonsoft.Json;

namespace HodlWallet2.Core.Models
{
    public class CurrencyEntity
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rate")]
        public float Rate { get; set; }
    }
}
