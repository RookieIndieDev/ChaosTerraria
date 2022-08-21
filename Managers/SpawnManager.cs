using ChaosTerraria.AI;
using ChaosTerraria.Classes;
using ChaosTerraria.NPCs;
using ChaosTerraria.TileEntities;
using ChaosTerraria.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ChaosTerraria.Managers
{
    public static class SpawnManager
    {
        private static int activeBotCount;
        private static int adamZeroCount;
        private static int numOfAdamZero = 5;
        private static int spawnCount;
        private static int totalSpawned;
        public static int ActiveBotCount { get => activeBotCount; set => activeBotCount = value; }
        public static int AdamZeroCount { get => adamZeroCount; set => adamZeroCount = value; }
        public static int NumOfAdamZero { get => numOfAdamZero; set => numOfAdamZero = value; }
        public static int SpawnCount { get => spawnCount; set => spawnCount = value; }
        public static int TotalSpawned { get => totalSpawned; set => totalSpawned = value; }

        public static void SpawnTerrarians()
        {
#if DEBUG
            if (AdamZeroCount == 0)
            {
                for (int i = 0; i < NumOfAdamZero; i++)
                {
                    foreach (Point spawnPoint in ChaosSystem.spawnBlocks)
                    {
                        int tileEntityIndex = ModContent.GetInstance<SpawnBlockTileEntity>().Find(spawnPoint.X, spawnPoint.Y);
                        if (tileEntityIndex != -1)
                        {
                            SpawnBlockTileEntity tileEntity = (SpawnBlockTileEntity)TileEntity.ByID[tileEntityIndex];
                            if (tileEntity.roleNamespace == "AdamZero" && tileEntity.spawnedSoFar < tileEntity.spawnCount)
                            {
                                var index = NPC.NewNPC(null, spawnPoint.X * 16, spawnPoint.Y * 16, NPCType<AdamZero>(), 1);
                                AdamZero adamZero = (AdamZero)Main.npc[index].ModNPC;
                                adamZero.NPC.GivenName += " " + i;
                                adamZero.spawnBlockTileEntity = tileEntity;
                                if (SessionManager.ObservableNPCs != null)
                                    SessionManager.ObservableNPCs.Add(adamZero);
                                AdamZeroCount++;
                                tileEntity.spawnedSoFar++;
                                break;
                            }
                        }
                    }
                }
            }
#endif
            if (ChaosSystem.spawnBlocks.Count > 0)
            {
                if (ActiveBotCount == 0)
                {

                    if (TotalSpawned == SessionManager.Organisms.Count)
                    {
                        ES.UpdateWeights();
                        TotalSpawned = 0;
                        SessionManager.PercentCompleted = 0;
                    }

                    if (SessionManager.Organisms != null)
                    {
                        foreach (Organism organism in SessionManager.Organisms)
                        {
                            if (organism.assigned == false)
                            {
                                foreach (Point spawnPoint in ChaosSystem.spawnBlocks)
                                {
                                    int tileEntityIndex = ModContent.GetInstance<SpawnBlockTileEntity>().Find(spawnPoint.X, spawnPoint.Y);
                                    if (tileEntityIndex != -1)
                                    {
                                        SpawnBlockTileEntity tileEntity = (SpawnBlockTileEntity)TileEntity.ByID[tileEntityIndex];
                                        if (tileEntity.roleNamespace == organism.trainingRoomRoleNamespace)
                                        {
                                            if (tileEntity.spawnCount == -1)
                                            {
                                                var index = NPC.NewNPC(null, spawnPoint.X * 16, spawnPoint.Y * 16, NPCType<ChaosTerrarian>(), 1);
                                                ChaosTerrarian terrarian = (ChaosTerrarian)Main.npc[index].ModNPC;
                                                if (SessionManager.ObservableNPCs != null)
                                                    SessionManager.ObservableNPCs.Add(terrarian);
                                                terrarian.spawnBlockTileEntity = null;
                                                terrarian.organism = SessionManager.GetOrganism(organism.trainingRoomRoleNamespace);
                                                ActiveBotCount++;
                                                break;
                                            }
                                            else if (tileEntity.spawnedSoFar < tileEntity.spawnCount)
                                            {
                                                var index = NPC.NewNPC(null, spawnPoint.X * 16, spawnPoint.Y * 16, NPCType<ChaosTerrarian>(), 1);
                                                ChaosTerrarian terrarian = (ChaosTerrarian)Main.npc[index].ModNPC;
                                                if (SessionManager.ObservableNPCs != null)
                                                    SessionManager.ObservableNPCs.Add(terrarian);
                                                terrarian.spawnBlockTileEntity = tileEntity;
                                                terrarian.organism = SessionManager.GetOrganism(organism.trainingRoomRoleNamespace);
                                                ActiveBotCount++;
                                                tileEntity.spawnedSoFar++;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
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
