using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosTerraria.Structs
{
    public struct Package
    {
        public TrainingRoom trainingRoom;
        public List<Role> roles;
        //public List<FitnessRule> fitnessRulesRaw;

    }

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
