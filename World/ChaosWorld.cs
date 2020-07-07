using ChaosTerraria.Managers;
using ChaosTerraria.Network;
using ChaosTerraria.Tiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace ChaosTerraria.World
{
    class ChaosWorld : ModWorld
    {
        private static List<SpawnBlock> spawnBlocks;
        public override void Initialize()
        {
            spawnBlocks = new List<SpawnBlock>();
        }

        public override void PostUpdate()
        {
            SpawnManager.SpawnTerrarians();
            ScanForSpawnBlocks();
        }

        public static void AddToSpawnPoints(SpawnBlock spawnBlock)
        {
            foreach (SpawnBlock block in spawnBlocks)
            {
                if (Main.tile[spawnBlock.BlockPos.X, spawnBlock.BlockPos.Y].isTheSameAs(Main.tile[block.BlockPos.X, block.BlockPos.Y]))
                {
                    return;
                }
            }
            spawnBlocks.Add(spawnBlock);
        }

        public static SpawnBlock GetRandomSpawnBlock()
        {
            if (spawnBlocks.Count > 0)
            {
                int index = WorldGen.genRand.Next(0, spawnBlocks.Count);
                return spawnBlocks.ElementAt(index);
            }
            else
            {
                return null;
            }
        }

        public static int GetSpawnBlockCount()
        {
            return spawnBlocks.Count();
        }

        private void ScanForSpawnBlocks()
        {
            int startX = Main.spawnTileX;
            int startY = Main.spawnTileY;
            const int range = 50;

            for (int i = startX - range; i < startX + range; i++)
            {
                for (int j = startY - range; j < startY + range; j++)
                {
                    if (Main.tile[i, j].type == ModContent.TileType<SpawnBlock>())
                    {
                        SpawnBlock spawnBlock = new SpawnBlock();
                        spawnBlock.BlockPos.X = i;
                        spawnBlock.BlockPos.Y = j;
                        AddToSpawnPoints(spawnBlock);
                    }
                }
            }
        }
    }
}
