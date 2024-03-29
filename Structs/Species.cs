﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChaosTerraria.Structs
{
    public struct Species
    {
        public string trainingRoomNamespace;
        [JsonProperty("namespace")]
        public string nameSpace;
        public string trankClass;
        public string parentNamespace;
        [JsonProperty("owner_username")]
        public string ownerUsername;
        public int age;
        public int currScore;
        public int childrenSpawnedThisGen;
        public int childrenReportedThisGen;
        public int gensSinceLastImprovement;
        public int highScore;
        public string observedAttributesRaw;
        public int generation;
        public string historicalScoresRaw;
        public List<HistoricalScores> historicalScores;
    }
}
