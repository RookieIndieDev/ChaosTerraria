using ChaosTerraria.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ChaosTerraria.TileEntities
{
    class SpawnBlockTileEntity : ModTileEntity
    {
        internal string roleNamespace = "default";
        internal int spawnCount = 1;
        internal int spawnedSoFar;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.IsActive && tile.type == ModContent.TileType<SpawnBlock>() && tile.frameX == 0 && tile.frameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            return Place(i, j);
        }

        public override void SaveData(TagCompound tag)
        {
            tag = new TagCompound
            {
                {"roleNamespace", roleNamespace},
                {"spawnCount", spawnCount}

            };
        }

        public override void LoadData(TagCompound tag)
        {
            roleNamespace = tag.Get<string>("roleNamespace");
            spawnCount = tag.GetAsInt("spawnCount");
        }
    }
}
