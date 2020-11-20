using Newtonsoft.Json;

namespace ChaosTerraria.Structs
{
    public struct Report
    {
        [JsonProperty("namespace")]
        public string nameSpace;
        public int score;
    }
}
