using ChaosTerraria.Network;
using ChaosTerraria.NPCs;
using ChaosTerraria.Tiles;
using ChaosTerraria.World;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Threading.Tasks;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace ChaosTerraria.Managers
{
	//TODO: Modify to work with roles
	public static class SpawnManager
	{
		public static int spawned = 0;
		public static int totalSpawned = 0;
		private static ChaosNetworkHelper networkHelper = new ChaosNetworkHelper();

		public static void SpawnTerrarians()
		{
			int spawnCount = 5;
			Point blockPos = new Point();
			if (spawned == 0 && /*SessionManager.CurrentSession.nameSpace != null*/ SessionManager.SessionStarted)
			{
				networkHelper.DoSessionNext();
				for (int i = 0; i < spawnCount; i++)
				{
					//if (spawnBlock != null)
					if(ChaosWorld.spawnBlocks.Count > 0)
					{
						blockPos = ChaosWorld.spawnBlocks.ElementAt<Point>(WorldGen.genRand.Next(0, ChaosWorld.spawnBlocks.Count));
					}
					else
					{
						Main.NewText("No Spawn Blocks Found! Place Spawn Blocks in the world!", Color.Red);
						return;
					}

					if (blockPos != null)
					{
						//NPC.NewNPC(spawnBlock.BlockPos.X * 16 , spawnBlock.BlockPos.Y * 16, NPCType<ChaosTerrarian>(), 1);
						NPC.NewNPC(blockPos.X * 16, blockPos.Y * 16, NPCType<ChaosTerrarian>(), 1); ;
						spawned++;
						totalSpawned++;
					}

				}
			}
		}
	}
}
