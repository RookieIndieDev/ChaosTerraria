using ChaosTerraria.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ChaosTerraria.Tile_Entities
{
    class SpawnBlockTileEntity : ModTileEntity
    {
        internal string roleNamespace = "Default";
        internal int spawnCount = 1;
        internal int spawnedSoFar;

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            return Place(i, j);
        }

        public override void Load(TagCompound tag)
        {
            roleNamespace = tag.Get<string>("roleNamespace");
            spawnCount = tag.GetAsInt("spawnCount");
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                {"roleNamespace", roleNamespace},
                {"spawnCount", spawnCount}

            };
        }

        public override void Update()
        {

        }

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == ModContent.TileType<SpawnBlock>() && tile.frameX == 0 && tile.frameY == 0;
        }
    }
}
