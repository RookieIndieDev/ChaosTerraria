using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChaosTerraria.Classes
{
    public class Role
    {
        public string name;
        //public List<Setting> settings;
        public List<string> inventory;
        public int count;
        public int lifeSeconds;
        public string craftItemOne;
        public string craftItemTwo;
        public string craftItemThree;
        public string craftItemFour;
        public string craftItemFive;
        public bool friendly;
    }
}
