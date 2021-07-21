﻿using ChaosTerraria.Enums;
using ChaosTerraria.Managers;
using ChaosTerraria.NPCs;
using ChaosTerraria.Structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ChaosTerraria.Fitness
{
    //TODO: Create EventHandler for Fitness Event
    //TODO: Implement lifeEffect
    //TODO: Change Postitions in TestMoveAlongAxis() to tileCoords?
    public static class FitnessManager
    {
        public static List<FitnessRule> fitnessRules;
        private static FitnessRuleType type;

        public static int TestFitness(ChaosTerrarian org, int minedTileType, Tile placedTile)
        {
            int score = 0;
            if (org.organism != null)
            {
                foreach (Role role in SessionManager.Package.roles)
                {
                    if (role.nameSpace == org.organism.trainingRoomRoleNamespace)
                    {
                        fitnessRules = JsonConvert.DeserializeObject<List<FitnessRule>>(role.fitnessRulesRaw);
                        break;
                    }
                }

                foreach (FitnessRule rule in fitnessRules)
                {
                    Enum.TryParse(rule.eventType, out type);
                    switch (type)
                    {
                        case FitnessRuleType.MOVE_ALONG_AXIS:
                            score += TestMoveAlongAxis(rule.attributeValue.ToLower(), org, rule.scoreEffect);
                            break;
                        case FitnessRuleType.BLOCK_MINED:
                            score += TestBlockMined(rule.attributeValue, minedTileType, rule.scoreEffect);
                            break;
                        case FitnessRuleType.BLOCK_PLACED:
                            score += TestBlockPlaced(rule.attributeValue, placedTile, rule.scoreEffect);
                            break;
                        default:
                            break;
                    }
                }
            }
            return score;
        }

        private static int TestBlockPlaced(string blockId, Tile placedTile, int scoreEffect)
        {
            int score = 0;

            if(placedTile != null){
                ;
                if (TileID.GetUniqueKey(placedTile.type).Split(' ')[1] == blockId)
                {
                    score += scoreEffect;
                }
            }

            return score;
        }

        private static int TestBlockMined(String blockId, int minedTileType, int scoreEffect)
        {
            int score = 0;

            if(minedTileType != -1)
            {
                
                if (TileID.GetUniqueKey(minedTileType).Split(' ')[1] == blockId)
                {
                    score += scoreEffect;
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
                    }

                    break;
                case "-x":
                    if (org.npc.position.X < org.npc.oldPosition.X)
                    {
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
