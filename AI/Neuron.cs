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
        public double weight;
        public string activator;
        public Dependency[] dependencies;
        public string id;
        public string direction;
        [JsonIgnore]
        public double value;
        [JsonIgnore]
        public bool evaluated;
    }
}
