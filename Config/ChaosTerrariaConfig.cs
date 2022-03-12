using ChaosTerraria.Classes;
using System;
using System.Collections.Generic;
using Terraria.ModLoader.Config;

namespace ChaosTerraria.Config
{
    public class ChaosTerrariaConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Example Fitness Rules")]
        [Tooltip("List of example of fitness rules to use with your agent")]
        public readonly List<FitnessRule> exampleFitnessRules = new List<FitnessRule>()
        {
            new ()
            {
                eventType = "MOVE_ALONG_AXIS",
                scoreEffect = 5,
                lifeEffect = 5,
                maxOccurrences = -1,
                attributeValue = "X"
            }, new (){
                eventType = "BLOCK_PLACED",
                scoreEffect = 5,
                lifeEffect = 5,
                maxOccurrences = -1,
                attributeValue = "Wood"
            }
        };
        [Label("Actual fitness rules")]
        [Tooltip("Add your fitness rules here for your agent to train with")]
        public List<FitnessRule> fitnessRules;
    }
}
