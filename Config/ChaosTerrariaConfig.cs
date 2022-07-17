using ChaosTerraria.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace ChaosTerraria.Config
{
    public class ChaosTerrariaConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Example Fitness Rules")]
        [Tooltip("Example of fitness rules to train your agent.")]
        public readonly List<FitnessRule> exampleFitnessRules = new()
        {
            new ()
            {
                eventType = "MOVE_ALONG_AXIS",
                scoreEffect = 5,
                lifeEffect = 5,
                maxOccurrences = -1,
                attributeValue = "X",
                roleName="RoleOne"
            }, new (){
                eventType = "BLOCK_PLACED",
                scoreEffect = 5,
                lifeEffect = 5,
                maxOccurrences = -1,
                attributeValue = "Wood",
                roleName="RoleTwo"
            }
        };
        [Label("Exmaple Roles")]
        [Tooltip("Example roles for reference")]
        public readonly List<Role> exampleRoles = new()
        {
            new()
            {
                name = "RoleOne",
                inventory = new List<string>{"Wood@20", "StoneBlock@30"},
                count = 1,
                craftItemOne = "Torch",
                craftItemTwo = "IronBar",
                craftItemThree = "WoodenBow",
                craftItemFour = "IronAnvil",
                craftItemFive = "Chest",
                friendly = true
            },
            new()
            {
                name = "RoleTwo",
                inventory = new List<string>{"CopperOre@20", "IronAxe@30"},
                count = 2,
                friendly = false
            }
        };
        [Label("Actual fitness rules")]
        [Tooltip("Add your fitness rules here for your agent to train with")]
        public List<FitnessRule> fitnessRules;
        [Label("Roles")]
        [Tooltip("Allow you configure various things such as inventory, etc., Look at Example Roles for reference")]
        public List<Role> roles;
        [Label("Noise Standard Deviation")]
        [Tooltip("Standard deviation of the noise added to the weights training. With default values, noise should follow std normal dist")]
        [DefaultValue(1f)]
        [Range(-10f,10f)]
        public float std;
        [Label("Noise Mean")]
        [Tooltip("Mean of the noise added to the weights during training. With default values, noise should follow std normal dist")]
        [DefaultValue(0.0f)]
        [Range(-10f, 10f)]
        public float mean;
        [Label("Learning Rate")]
        [DefaultValue(0.01f)]
        public float learningRate;
    }
}
