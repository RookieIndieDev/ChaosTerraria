using System.Collections.Generic;

namespace ChaosTerraria.Structs
{
    public struct TrainingRoomSessionNextRequest
    {
        public bool nNetRaw;
        public List<Report> report;
        public List<ObservedAttributes> observedAttributes;
    }
}
