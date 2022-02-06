using ChaosTerraria.TileEntities;
using ChaosTerraria.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ChaosTerraria.UI;

namespace ChaosTerraria.Tiles
{
    //Modify to store spawn point ID
    class SpawnBlock : ModTile
    {
        SpawnBlockTileEntity tileEntity;
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            foreach(Point block in ChaosSystem.spawnBlocks)
            {
                if(block.X == i && block.Y == j && fail)
                {
                    ChaosSystem.spawnBlocks.Remove(block);
                    break;
                }
            }
        }

        public override bool RightClick(int i, int j)
        {
            UIHandler.ShowSpawnBlockScreen(i, j);
            return true;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            //ChaosWorld.AddToSpawnPoints(this);
            ChaosSystem.spawnBlocks.Add(new Point(i, j));
            tileEntity.Place(i, j);
            Main.NewText("Spawn Block added!" + " Total Spawn Blocks: " + ChaosSystem.GetSpawnBlockCount());
        }



        public override void SetStaticDefaults()
        {
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<SpawnBlockTileEntity>().Hook_AfterPlacement, -1, 0, true);
            tileEntity = ModContent.GetInstance<SpawnBlockTileEntity>();
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;   
            Main.tileLighted[Type] = true;
            DustType = 84;
            ItemDrop = ModContent.ItemType<Items.SpawnBlock>();
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
