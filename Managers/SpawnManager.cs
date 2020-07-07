using ChaosTerraria.Config;
using ChaosTerraria.NPCs;
using ChaosTerraria.Tiles;
using ChaosTerraria.World;
using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace ChaosTerraria.Managers
{
	public static class SpawnManager
	{
		public static int spawned = 0;
		public static int totalSpawned = 0;

		public static void SpawnTerrarians()
		{
			int spawnCount = GetInstance<ChaosConfig>().numberOfOrgs;
			if (spawned == 0)
			{
				for (int i = 0; i < spawnCount; i++)
				{
					SpawnBlock spawnBlock = ChaosWorld.GetRandomSpawnBlock();
					if (spawnBlock != null)
					{
						NPC.NewNPC(spawnBlock.BlockPos.X * 16, spawnBlock.BlockPos.Y * 16, NPCType<ChaosTerrarian>(), 1);
						spawned++;
						totalSpawned++;
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
}
