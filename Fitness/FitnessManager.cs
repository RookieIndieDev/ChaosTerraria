﻿using ChaosTerraria.Enums;
using ChaosTerraria.NPCs;
using ChaosTerraria.Structs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace ChaosTerraria.Fitness
{
    //TODO: Create EventHandler for Fitness Event
    //TODO: Change to work with different rules for different roles
    //TODO: Implement lifeEffect
    //TODO: Change Postitions in TestMoveAlongAxis() to tileCoords?
    public static class FitnessManager
    {
        public static List<FitnessRule> fitnessRules;
        private static FitnessRuleType type;

        public static int TestFitness(ChaosTerrarian org)
        {
            int score = 0;
            foreach (FitnessRule rule in fitnessRules)
            {
                Enum.TryParse(rule.eventType, out type);
                switch (type)
                {
                    case FitnessRuleType.MOVE_ALONG_AXIS:
                        score += TestMoveAlongAxis(rule.attributeValue.ToLower(), org, rule.scoreEffect);
                        break;
                    default:
                        break;
                }
            }
            return score;
        }
        
        private static int TestMoveAlongAxis(String axis, ChaosTerrarian org, int scoreEffect)
        {
            int score = 0;
            switch (axis)
            {
                case "x":
                    if (org.npc.position.X > org.npc.oldPosition.X)
                    {
                        score += scoreEffect;
                        Dust.NewDust(org.npc.position, 10, 10, 5, newColor: Color.HotPink);
                    }

                    break;
                case "-x":
                    if (org.npc.position.X < org.npc.oldPosition.X)
                    {
                        Dust.NewDust(org.npc.position, 10, 10, 5, newColor: Color.HotPink);
                        score += scoreEffect;
                    }
                    break;
                default:
                    throw new Exception("Invalid Axis Value for this rule!");
            }
            return score;
        }
    }
}
