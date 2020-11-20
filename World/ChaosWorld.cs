using ChaosTerraria.Managers;
using ChaosTerraria.Network;
using ChaosTerraria.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace ChaosTerraria.World
{
    class ChaosWorld : ModWorld
    {
        public static HashSet<Point> spawnBlocks;
        ChaosNetworkHelper networkHelper = new ChaosNetworkHelper();

        public override void Initialize()
        {
            spawnBlocks = new HashSet<Point>();
        }


        //TODO: Don't make call to StartSession if namespace already exists?
        public override void PostUpdate()
        {
            if (!SessionManager.SessionStarted)
            {

                if (SessionManager.CurrentSession.nameSpace != null)
                {
                    networkHelper.StartSession();
                    SessionManager.SessionStarted = true;
                }
            }
            ScanForSpawnBlocks();
            SpawnManager.SpawnTerrarians();
        }

/*        public static void AddToSpawnPoints(SpawnBlock spawnBlock)
        {
            *//*            foreach (SpawnBlock block in spawnBlocks)
                        {
                            if (Main.tile[spawnBlock.BlockPos.X, spawnBlock.BlockPos.Y].isTheSameAs(Main.tile[block.BlockPos.X, block.BlockPos.Y]))
                            {
                                return;
                            }
                        }*//*
            spawnBlocks.Add(spawnBlock);
        }*/

/*        public static SpawnBlock GetRandomSpawnBlock()
        {
            if (spawnBlocks.Count > 0)
            {
                return spawnBlocks.ElementAt(WorldGen.genRand.Next(0, spawnBlocks.Count));
            }
            else
            {
                return null;
            }
        }*/

/*         public static Point GetRandomSpawnBlock()
        {
            if(spawnBlocks.Count > 0)
            {
                return spawnBlocks.ElementAt(WorldGen.genRand.Next(0, spawnBlocks.Count));
            }
            else
            {
                return null;
            }
        }
*/
        public static int GetSpawnBlockCount()
        {
            return spawnBlocks.Count();
        }

        private void ScanForSpawnBlocks()
        {
            int startX = Main.spawnTileX;
            int startY = Main.spawnTileY;
            const int range = 100;

            for (int i = startX - range; i < startX + range; i++)
            {
                for (int j = startY - range; j < startY + range; j++)
                {
                    if (Framing.GetTileSafely(i, j).type == ModContent.TileType<SpawnBlock>())
                    {
                        SpawnBlock spawnBlock = new SpawnBlock();
                        spawnBlock.BlockPos.X = i;
                        spawnBlock.BlockPos.Y = j;
                        //AddToSpawnPoints(new Point(i, j));
                        spawnBlocks.Add(new Point(i, j));
                    }
                }
            }
        }
    }
}
