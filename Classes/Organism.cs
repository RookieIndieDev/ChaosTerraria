using ChaosTerraria.AI;
using Newtonsoft.Json;

namespace ChaosTerraria.Classes
{
    public class Organism
    {
        [JsonProperty("namespace")]
        public string nameSpace;
        public string name;
        [JsonProperty("owner_username")]
        public string ownerUsername;
        public string trainingRoomNamespace;
        public string trainingRoomRoleNamespace;
        public string speciesNamespace;
        public string parentNamespace;
        public NNet nNet;
        public string nNetRaw;
        public int generation;
        [JsonIgnore]
        public bool assigned;
    }
}
