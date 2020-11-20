using ChaosTerraria.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ChaosTerraria.Tiles
{
    //Modify to store spawn point ID
    class SpawnBlock : ModTile
    {
        public Point BlockPos;
        public override void PlaceInWorld(int i, int j, Item item)
        {
            //ChaosWorld.AddToSpawnPoints(this);
            ChaosWorld.spawnBlocks.Add(BlockPos);
            BlockPos.X = i;
            BlockPos.Y = j;
            Main.NewText("Spawn Block added!" + " Total Spawn Blocks: " + ChaosWorld.GetSpawnBlockCount());
        }

        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            dustType = mod.DustType("Sparkle");
            drop = mod.ItemType("SpawnBlock");
            AddMapEntry(new Color(125, 41, 162));
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Spawn Block");
            AddMapEntry(new Color(160, 49, 209), name);
        }
    }
}
