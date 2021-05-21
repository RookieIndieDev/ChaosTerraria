using ChaosTerraria.Classes;
using ChaosTerraria.Network;
using ChaosTerraria.NPCs;
using ChaosTerraria.Tile_Entities;
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
		public static int spawned;
        public static int adamZeroCount;
		public static int totalSpawned;
		public static int spawnCount;
		private static ChaosNetworkHelper networkHelper = new ChaosNetworkHelper();
		static int timer;

		public static void SpawnTerrarians()
		{
			timer++;
			Point blockPos;
            if (adamZeroCount == 0 && SessionManager.AdamZeroEnabled)
            {
                foreach (Point spawnPoint in ChaosWorld.spawnBlocks)
                {
                    int tileEntityIndex = ModContent.GetInstance<SpawnBlockTileEntity>().Find(spawnPoint.X, spawnPoint.Y);
                    if (tileEntityIndex != -1)
                    {
                        SpawnBlockTileEntity tileEntity = (SpawnBlockTileEntity)TileEntity.ByID[tileEntityIndex];
                        if (tileEntity.roleNamespace == "AdamZero")
                        {
                            NPC.NewNPC(spawnPoint.X * 16, spawnPoint.Y * 16, NPCType<AdamZero>(), 1);
                            adamZeroCount++;
                        }
                    }
                }
            }
			if (spawned == 0 && SessionManager.SessionStarted)
			{
				if (ChaosWorld.spawnBlocks.Count > 0)
				{
					if (timer > 300)
					{
						networkHelper.DoSessionNext();
						timer = 0;
					}

					/*					for (int i = 0; i < spawnCount; i++)
										{
											blockPos = ChaosWorld.spawnBlocks.ElementAt<Point>(WorldGen.genRand.Next(0, ChaosWorld.spawnBlocks.Count));
											if (blockPos != null)
											{
												var index = NPC.NewNPC(blockPos.X * 16, blockPos.Y * 16, NPCType<ChaosTerrarian>(), 1);
												ChaosTerrarian terrarian = (ChaosTerrarian)Main.npc[index].modNPC;
												spawned++;
												totalSpawned++;
											}
										}*/
					if (SessionManager.Organisms != null)
					{
						foreach (Organism organism in SessionManager.Organisms)
						{
							foreach (Point spawnPoint in ChaosWorld.spawnBlocks)
							{
								int tileEntityIndex = ModContent.GetInstance<SpawnBlockTileEntity>().Find(spawnPoint.X, spawnPoint.Y);
								if (tileEntityIndex != -1)
								{
									SpawnBlockTileEntity tileEntity = (SpawnBlockTileEntity)TileEntity.ByID[tileEntityIndex];
									if (tileEntity.roleNamespace == organism.trainingRoomRoleNamespace)
									{
										var index = NPC.NewNPC(spawnPoint.X * 16, spawnPoint.Y * 16, NPCType<ChaosTerrarian>(), 1);
										ChaosTerrarian terrarian = (ChaosTerrarian)Main.npc[index].modNPC;
										terrarian.organism = organism;
										spawned++;
										break;
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
}
