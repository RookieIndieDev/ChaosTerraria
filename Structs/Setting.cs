using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosTerraria.Structs
{
    public struct Setting
    {
        [JsonProperty("namespace")]
        public string nameSpace;
        public String value;
        public string type;
    }
}
