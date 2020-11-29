using ChaosTerraria.Tile_Entities;
using ChaosTerraria.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ChaosTerraria.UI;
using System;

namespace ChaosTerraria.Tiles
{
    //Modify to store spawn point ID
    class SpawnBlock : ModTile
    {
        SpawnBlockTileEntity tileEntity;
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            foreach(Point block in ChaosWorld.spawnBlocks)
            {
                if(block.X == i && block.Y == j && fail)
                {
                    ChaosWorld.spawnBlocks.Remove(block);
                    break;
                }
            }
        }

        public override bool NewRightClick(int i, int j)
        {
            UIHandler.ShowSpawnBlockScreen(i, j);
            return true;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            //ChaosWorld.AddToSpawnPoints(this);
            ChaosWorld.spawnBlocks.Add(new Point(i, j));
            tileEntity.Place(i, j);
            Main.NewText("Spawn Block added!" + " Total Spawn Blocks: " + ChaosWorld.GetSpawnBlockCount());
        }



        public override void SetDefaults()
        {
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<SpawnBlockTileEntity>().Hook_AfterPlacement, -1, 0, true);
            tileEntity = ModContent.GetInstance<SpawnBlockTileEntity>();
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
