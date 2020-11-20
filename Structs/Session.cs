using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace ChaosTerraria.Structs
{
    public struct Session
    {
        public List<String> organisms;
        public string lastUpdateDate;
        public string state;
        public string _id;
        public string trainingRoomNamespace;
        public string trainingRoomUsername;
        public string trainingRoomId;
        [JsonProperty("namespace")]
        public string nameSpace;
        [JsonProperty("owner_username")]
        public string ownerUsername;
        public List<Setting> settings;
        public string __v;
    }
}
