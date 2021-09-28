using ChaosTerraria.Classes;
using ChaosTerraria.Network;
using ChaosTerraria.NPCs;
using ChaosTerraria.TileEntities;
using ChaosTerraria.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ChaosTerraria.Managers
{
    public static class SpawnManager
    {
        public static int activeBotCount;
        public static int adamZeroCount;
        public static int numOfAdamZero = 5;
        public static int spawnCount;
        public static int totalSpawned;
        private static ChaosNetworkHelper networkHelper = new ChaosNetworkHelper();
        private static int timer;
        private static bool timerEnabled;

        public static void SpawnTerrarians()
        {
            if(timerEnabled)
                timer++;
#if DEBUG
            if (adamZeroCount == 0)
            {
                for (int i = 0; i < numOfAdamZero; i++)
                {
                    foreach (Point spawnPoint in ChaosWorld.spawnBlocks)
                    {
                        int tileEntityIndex = ModContent.GetInstance<SpawnBlockTileEntity>().Find(spawnPoint.X, spawnPoint.Y);
                        if (tileEntityIndex != -1)
                        {
                            SpawnBlockTileEntity tileEntity = (SpawnBlockTileEntity)TileEntity.ByID[tileEntityIndex];
                            if (tileEntity.roleNamespace == "AdamZero" && tileEntity.spawnedSoFar < tileEntity.spawnCount)
                            {
                                var index = NPC.NewNPC(spawnPoint.X * 16, spawnPoint.Y * 16, NPCType<AdamZero>(), 1);
                                AdamZero adamZero = (AdamZero)Main.npc[index].modNPC;
                                adamZero.npc.GivenName += " " + i;
                                adamZero.spawnBlockTileEntity = tileEntity;
                                adamZeroCount++;
                                tileEntity.spawnedSoFar++;
                                break;
                            }
                        }
                    }
                }
            }
#endif
            if (activeBotCount == 0 && SessionManager.SessionStarted)
            {
                if (ChaosWorld.spawnBlocks.Count > 0)
                {

                    if (SessionManager.Organisms != null)
                    {
                        foreach (Organism organism in SessionManager.Organisms)
                        {
                            if (organism.assigned == false)
                            {
                                foreach (Point spawnPoint in ChaosWorld.spawnBlocks)
                                {
                                    int tileEntityIndex = ModContent.GetInstance<SpawnBlockTileEntity>().Find(spawnPoint.X, spawnPoint.Y);
                                    if (tileEntityIndex != -1)
                                    {
                                        SpawnBlockTileEntity tileEntity = (SpawnBlockTileEntity)TileEntity.ByID[tileEntityIndex];
                                        if (tileEntity.roleNamespace == organism.trainingRoomRoleNamespace)
                                        {
                                            if (tileEntity.spawnCount == -1)
                                            {
                                                var index = NPC.NewNPC(spawnPoint.X * 16, spawnPoint.Y * 16, NPCType<ChaosTerrarian>(), 1);
                                                ChaosTerrarian terrarian = (ChaosTerrarian)Main.npc[index].modNPC;
                                                terrarian.spawnBlockTileEntity = null;
                                                activeBotCount++;
                                                break;
                                            }
                                            else if (tileEntity.spawnedSoFar < tileEntity.spawnCount)
                                            {
                                                var index = NPC.NewNPC(spawnPoint.X * 16, spawnPoint.Y * 16, NPCType<ChaosTerrarian>(), 1);
                                                ChaosTerrarian terrarian = (ChaosTerrarian)Main.npc[index].modNPC;
                                                terrarian.spawnBlockTileEntity = tileEntity;
                                                activeBotCount++;
                                                tileEntity.spawnedSoFar++;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                        }

                        if(timer == 3600 && totalSpawned == 0)
                        {
                            timer = 0;
                            totalSpawned = spawnCount;
                        }

                        if (spawnCount != 0 && spawnCount == totalSpawned)
                        {
                            networkHelper.DoSessionNext();
                            totalSpawned = 0;
                            timerEnabled = !timerEnabled;
                        }
                    }
                }
                else
                {
                    Main.NewText("No Spawn Blocks Found! Place Spawn Blocks in the world!", Color.Red);
                    return;
                }
            }
        }
    }
}
