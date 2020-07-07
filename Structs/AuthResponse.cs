using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosTerraria.Structs
{
    public struct AuthResponse
    {
        public string accessToken;
        public string refreshToken;
        public int expiration;
        public int issuedAt;
        public string idToken;
    }
}
