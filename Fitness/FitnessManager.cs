using ChaosTerraria.Enums;
using ChaosTerraria.NPCs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using ChaosTerraria.Classes;

namespace ChaosTerraria.Fitness
{
    //TODO: Change Postitions in TestMoveAlongAxis() to tileCoords?
    public class FitnessManager
    {
        private List<FitnessRule> fitnessRules;
        private FitnessRuleType type;

        public FitnessManager(List<FitnessRule> rules)
        {
            fitnessRules = new List<FitnessRule>(rules);
        }

        public int TestFitness(ChaosTerrarian org, int minedTileType, Tile placedTile, string craftedItem, out int lifeEffect)
        {
            int score = 0;
            int tempLifeEffect = 0;
            int tempScore;
            if (fitnessRules != null)
            {
                foreach (FitnessRule rule in fitnessRules)
                {
                    tempScore = 0;
                    Enum.TryParse(rule.eventType, out type);
                    switch (type)
                    {
                        case FitnessRuleType.MOVE_ALONG_AXIS:

                            if (rule.maxOccurrences == -1)
                            {

                                tempScore += TestMoveAlongAxis(rule.attributeValue.ToLower(), org, rule.scoreEffect);
                                if (tempScore != 0)
                                {
                                    tempLifeEffect += rule.lifeEffect;
                                    score += tempScore;
                                }

                            }
                            else if(rule.maxOccurrences > 0)
                            {
                                tempScore += TestMoveAlongAxis(rule.attributeValue.ToLower(), org, rule.scoreEffect);
                                if (tempScore != 0)
                                {
                                    rule.maxOccurrences--;
                                    tempLifeEffect += rule.lifeEffect;
                                    score += tempScore;
                                }

                            }
                            break;
                        case FitnessRuleType.BLOCK_MINED:
                            if (rule.maxOccurrences == -1)
                            {
                                tempScore += TestBlockMined(rule.attributeValue, minedTileType, rule.scoreEffect);
                                if (tempScore != 0)
                                {
                                    tempLifeEffect += rule.lifeEffect;
                                    score += tempScore;
                                }

                            }
                            else if(rule.maxOccurrences > 0)
                            {
                                tempScore += TestBlockMined(rule.attributeValue, minedTileType, rule.scoreEffect);
                                if (tempScore != 0)
                                {
                                    rule.maxOccurrences--;
                                    tempLifeEffect += rule.lifeEffect;
                                    score += tempScore;
                                }
                            }
                            break;
                        case FitnessRuleType.BLOCK_PLACED:
                            if(rule.maxOccurrences == -1)
                            {
                                tempScore += TestBlockPlaced(rule.attributeValue, placedTile, rule.scoreEffect);
                                if (tempScore != 0)
                                {
                                    tempLifeEffect += rule.lifeEffect;
                                    score += tempScore;
                                }
                            }
                            else if(rule.maxOccurrences > 0)
                            {
                                tempScore += TestBlockPlaced(rule.attributeValue, placedTile, rule.scoreEffect);
                                if (tempScore != 0)
                                {
                                    rule.maxOccurrences--;
                                    tempLifeEffect += rule.lifeEffect;
                                    score += tempScore;
                                }
                            }
                            break;
                        case FitnessRuleType.ITEM_CRAFTED:
                            if (rule.maxOccurrences == -1)
                            {
                                tempScore += TestItemCrafted(rule.attributeValue, craftedItem, rule.scoreEffect);
                                if (tempScore != 0)
                                {
                                    tempLifeEffect += rule.lifeEffect;
                                    score += tempScore;
                                }
                            }
                            else if (rule.maxOccurrences > 0)
                            {
                                tempScore += TestItemCrafted(rule.attributeValue, craftedItem, rule.scoreEffect);
                                if (tempScore != 0)
                                {
                                    rule.maxOccurrences--;
                                    tempLifeEffect += rule.lifeEffect;
                                    score += tempScore;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            lifeEffect = tempLifeEffect;
            return score;
        }

        private int TestBlockPlaced(string blockId, Tile placedTile, int scoreEffect)
        {
            int score = 0;

            if (placedTile != null)
            {
                ;
                if (TileID.GetUniqueKey(placedTile.type).Split(' ')[1] == blockId)
                {
                    score += scoreEffect;
                }
            }

            return score;
        }

        private int TestBlockMined(String blockId, int minedTileType, int scoreEffect)
        {
            int score = 0;

            if (minedTileType != -1)
            {

                if (TileID.GetUniqueKey(minedTileType).Split(' ')[1] == blockId)
                {
                    score += scoreEffect;
                }
            }

            return score;
        }

        private int TestMoveAlongAxis(String axis, ChaosTerrarian org, int scoreEffect)
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

        private int TestItemCrafted(String expectedItem, string craftedItem, int scoreEffect)
        {
            int score = 0;
            if (craftedItem == expectedItem)
                score += scoreEffect;
            return score;
        }
    }
}
