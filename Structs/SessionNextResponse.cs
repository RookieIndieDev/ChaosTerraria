using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChaosTerraria.Classes;

namespace ChaosTerraria.Structs
{
    public struct SessionNextResponse 
    {
        public List<Organism> organisms;
        public Stats stats;
        public List<Species> species;
    }
}
