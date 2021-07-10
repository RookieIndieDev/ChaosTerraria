using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChaosTerraria.Structs
{
    public struct Role
    {
        [JsonProperty("namespace")]
        public string nameSpace;
        public string name;
        public string trainingRoomNamespace;
        public string trainingRoomUsername;
        [JsonProperty("owner_username")]
        public string ownerUsername;
        public List<Setting> settings;
        public string fitnessRulesRaw;
    }
}
