using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosTerraria.Structs
{
    public struct SessionNextInfo
    {
        [JsonProperty("TrainingRoomSessionNextRequest")]
        public TrainingRoomSessionNextRequest trainingRoomSessionNextRequest;
    }
}
