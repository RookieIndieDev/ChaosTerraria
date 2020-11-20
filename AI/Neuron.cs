using Newtonsoft.Json;

namespace ChaosTerraria.AI
{
    public class Neuron
    {
        [JsonProperty("$TYPE")]
        public string type;
        [JsonProperty("_base_type")]
        public string baseType;
        [JsonProperty("$DEFAULT")]
        public bool Default;
        public float weight;
        public string activator;
        public Dependency[] dependencies;
        public string id;
        [JsonIgnore]
        public float value;
    }
}
