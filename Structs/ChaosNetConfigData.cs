using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosTerraria.Structs
{
   public struct ChaosNetConfigData
    {
        public int maxBotCount;
        public string idToken;
        public int expiration;
        public string sessionNamespace;
        public string trainingRoomNamespace;
        public string trainingRoomUsernameNamespace;
        public string accessToken;
        public string env;
        public string username;
        public string refreshToken;
    }
}
