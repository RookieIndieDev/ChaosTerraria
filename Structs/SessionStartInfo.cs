using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosTerraria.Structs
{ 
    public struct SessionStartInfo
    {
        public string username;
        public string trainingroom;
        public TrainingRoomRequest TrainingRoomStartRequest;

        public struct TrainingRoomRequest
        {
            public bool reset;
        }
    }
}
