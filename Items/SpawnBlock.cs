using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ChaosTerraria.Items
{
    class SpawnBlock : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 1;
            Item.height = 1;
            Item.maxStack = 999;
            Item.createTile = TileType<Tiles.SpawnBlock>();
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Place block to have orgs spawn from");
            DisplayName.SetDefault("Spawn Block");
        }
    }
}
