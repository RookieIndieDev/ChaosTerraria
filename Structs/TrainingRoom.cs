using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChaosTerraria.Structs
{
    public struct TrainingRoom
    {
        [JsonProperty("namespace")]
        public string nameSpace;
        public string name;
        [JsonProperty("owner_username")]
        public string ownerUsername;
        public List<Setting> settings;
    }
}
