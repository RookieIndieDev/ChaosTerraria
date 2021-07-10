using Newtonsoft.Json;
using System;

namespace ChaosTerraria.Structs
{
    public struct Setting
    {
        [JsonProperty("namespace")]
        public string nameSpace;
        public String value;
        public string type;
    }
}
