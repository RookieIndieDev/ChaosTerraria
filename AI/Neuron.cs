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
        public string range;
        public string x;
        public string y;
        public string attributeId;
        public string attributeValue;
        [JsonIgnore]
        public double value;
        [JsonIgnore]
        public bool evaluated;
    }
}
