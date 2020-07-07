using System;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader.IO;
using Terraria;
using ChaosTerraria.World;
using System.Runtime.InteropServices;

namespace ChaosTerraria.Items
{
    class SpawnBlock : ModItem
    { 
        public override void SetDefaults()
        {
            item.width = 1;
            item.height = 1;
            item.maxStack = 999;
            item.createTile = TileType<Tiles.SpawnBlock>();
            item.consumable = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 15;
        }

        public override void AddRecipes()
        {
            ModRecipe spawnBlockRecipe = new ModRecipe(mod);
            spawnBlockRecipe.AddIngredient(ItemID.Wood);
            spawnBlockRecipe.SetResult(this, 999);
            spawnBlockRecipe.AddRecipe();
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Place block to have orgs spawn from");
            DisplayName.SetDefault("Spawn Block");
        }
    }
}
