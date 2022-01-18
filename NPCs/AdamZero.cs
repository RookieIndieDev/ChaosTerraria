using ChaosTerraria.Classes;
using ChaosTerraria.Managers;
using ChaosTerraria.Enums;
using ChaosTerraria.Tiles;
using ChaosTerraria.TileEntities;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Newtonsoft.Json;
using ChaosTerraria.AI;
using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace ChaosTerraria.NPCs
{
    //TODO: Add realistic block breaking
    //No of ticks = time in seconds * 60
    class AdamZero : ModNPC
    {
        public override string Texture => "ChaosTerraria/NPCs/Terrarian";
        private int timer;
        private int timeLeft;
        internal Organism organism;
        private int lifeTicks = 600;
        private List<Item> inventory = new List<Item>();
        internal SpawnBlockTileEntity spawnBlockTileEntity;
        int lastItemIndex;
        HitTile hitTile = new HitTile();
        Item axe = new Item();
        Item pickaxe = new Item();
        Item hammer = new Item();
        private int damage;
        private int tileId;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 700;
            NPCID.Sets.MustAlwaysDraw[npc.type] = true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            SpawnManager.adamZeroCount--;
            timeLeft = 0;
            npc.life = 0;
            spawnBlockTileEntity.spawnedSoFar--;
            if (SessionManager.ObservableNPCs != null && SessionManager.ObservableNPCs.Count > 0)
                SessionManager.ObservableNPCs.Remove(this);
            return true;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.townNPC = false;
            npc.friendly = true;
            npc.width = 18;
            npc.height = 40;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 100;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.Guide;
            npc.homeless = true;
            npc.noGravity = false;
            npc.dontTakeDamage = false;
#if DEBUG
            organism = new Organism
            {
                nNet = JsonConvert.DeserializeObject<NNet>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                + @"\My Games\Terraria\ModLoader\Mod Sources\ChaosTerraria\NNet.json")),
                nameSpace = "AdamZero",
                trainingRoomRoleNamespace = "AdamZero"
            };
#endif
            npc.GivenName = "AdamZero";
            inventory.Add(new Item());
            ItemID.Search.TryGetId("Wood", out int id);
            inventory[lastItemIndex].SetDefaults(id);
            inventory[lastItemIndex].stack = 10;
            lastItemIndex++;
            axe.SetDefaults(ItemID.CopperAxe);
            pickaxe.SetDefaults(ItemID.CopperPickaxe);
            hammer.SetDefaults(ItemID.CopperHammer);
        }

        public override void AI()
        {
            timer++;
            timeLeft++;
            if (timer > 18 && npc.active)
            {
                if (organism != null)
                {
                    int action = organism.nNet.GetOutput(npc.Center.ToTileCoordinates(), inventory, out int direction, out string itemToCraft, out string blockToPlace, out int x, out int y);
                    DoActions(action, direction, itemToCraft, blockToPlace, x, y);
                    UpdateInventory();
                }
                timer = 0;
            }
            if (timeLeft > lifeTicks && npc.active)
            {
                SpawnManager.adamZeroCount--;
                timeLeft = 0;
                npc.life = 0;
                spawnBlockTileEntity.spawnedSoFar--;
                if (SessionManager.ObservableNPCs != null && SessionManager.ObservableNPCs.Count > 0)
                    SessionManager.ObservableNPCs.Remove(this);
            }
        }

        Item FindInventoryItemStack(string name)
        {
            if (inventory != null)
            {
                foreach (Item item in inventory)
                {
                    if (item.Name == name && item.stack > 0)
                        return item;
                }
            }
            return null;
        }

        private void UpdateInventory()
        {
            if (inventory != null)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i].stack == 0)
                    {
                        inventory.Remove(inventory[i]);
                        lastItemIndex--;
                    }
                }
            }
        }

        private void MoveRight()
        {
            npc.velocity.X = 0.5f;
            npc.direction = 1;
        }

        private void MoveLeft()
        {
            npc.velocity.X = -0.5f;
            npc.direction = -1;
        }

        private void Jump()
        {
            if (npc.collideY)
            {
                npc.velocity.Y = -8f;
            }
        }

        private void PlaceBlockLeft(string blockToPlace, int x)
        {
            var pos = npc.Left.ToTileCoordinates();
            pos.X -= x;
            pos.Y = blockToPlace.Contains("Door") ? pos.Y : pos.Y + 1;
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).active() && (Framing.GetTileSafely(pos.X, pos.Y + 1).active() || Framing.GetTileSafely(pos.X, pos.Y - 1).active()))
            {
                if (blockToPlace.Contains("Door"))
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                }
                else
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                }
                item.stack--;
            }

            npc.direction = -1;
        }

        private void PlaceBlockRight(string blockToPlace, int x)
        {
            var pos = npc.Right.ToTileCoordinates();
            pos.X += x;
            pos.Y = blockToPlace.Contains("Door") ? pos.Y : pos.Y + 1;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).active() && (Framing.GetTileSafely(pos.X, pos.Y + 1).active() || Framing.GetTileSafely(pos.X, pos.Y - 1).active()))
            {
                if (blockToPlace.Contains("Door"))
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                }
                else
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                }
                item.stack--;
            }

            npc.direction = 1;
        }

        private void PlaceBlockBottomRight(string blockToPlace, int x, int y)
        {
            var pos = npc.BottomRight.ToTileCoordinates();
            pos.X += x;
            pos.Y += y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).active() && (Framing.GetTileSafely(pos.X, pos.Y + 1).active() || Framing.GetTileSafely(pos.X, pos.Y - 1).active()))
            {
                if (blockToPlace.Contains("Door"))
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                }
                else
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                }
                item.stack--;
            }
            npc.direction = 1;
        }

        private void PlaceBlockBottomLeft(string blockToPlace, int x, int y)
        {
            var pos = npc.BottomLeft.ToTileCoordinates();
            pos.X -= x;
            pos.Y += y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).active() && (Framing.GetTileSafely(pos.X, pos.Y + 1).active() || Framing.GetTileSafely(pos.X, pos.Y - 1).active()))
            {
                if (blockToPlace.Contains("Door"))
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                }
                else
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                }
                item.stack--;
            }

            npc.direction = -1;
        }

        private void PlaceBlockBottom(string blockToPlace, int y)
        {
            var pos = npc.Bottom.ToTileCoordinates();
            pos.Y += y;
            var item = FindInventoryItemStack(blockToPlace);
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).active() && (Framing.GetTileSafely(pos.X, pos.Y + 1).active() || Framing.GetTileSafely(pos.X, pos.Y - 1).active()))
            {
                if (blockToPlace.Contains("Door"))
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                }
                else
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                }
                item.stack--;
            }
        }

        private void PlaceBlockTopRight(string blockToPlace, int x, int y)
        {
            var pos = npc.TopRight.ToTileCoordinates();
            pos.X += x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).active() && (Framing.GetTileSafely(pos.X, pos.Y + 1).active() || Framing.GetTileSafely(pos.X, pos.Y - 1).active()))
            {
                if (blockToPlace.Contains("Door"))
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                }
                else
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                }
                item.stack--;
            }

            npc.direction = 1;
        }

        private void PlaceBlockTopLeft(string blockToPlace, int x, int y)
        {
            var pos = npc.TopLeft.ToTileCoordinates();
            pos.X -= x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).active() && (Framing.GetTileSafely(pos.X, pos.Y + 1).active() || Framing.GetTileSafely(pos.X, pos.Y - 1).active()))
            {
                if (blockToPlace.Contains("Door"))
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                }
                else
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                }
                item.stack--;
            }

            npc.direction = -1;
        }

        private void PlaceBlockTop(string blockToPlace, int y)
        {
            var pos = npc.Top.ToTileCoordinates();
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).active() && (Framing.GetTileSafely(pos.X, pos.Y + 1).active() || Framing.GetTileSafely(pos.X, pos.Y - 1).active()))
            {
                if (blockToPlace.Contains("Door"))
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    item.stack--;
                }
                else if (Framing.GetTileSafely(pos.X - 1, pos.Y).active() || Framing.GetTileSafely(pos.X + 1, pos.Y).active() || Framing.GetTileSafely(pos.X, pos.Y + 1).active())
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                    item.stack--;
                }
            }
        }

        private void MineBlockLeft(int range)
        {
            var pos = npc.Left.ToTileCoordinates();
            pos.X -= range;
            pos.Y++;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var tile = Framing.GetTileSafely(pos.X, pos.Y);
            if (tile.type != ModContent.TileType<SpawnBlock>() && tile.active())
            {
                HitTargetPos(pos);
            }
            npc.direction = -1;
        }

        private void MineBlockRight(int range)
        {
            var pos = npc.Right.ToTileCoordinates();
            pos.X += range;
            pos.Y++;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var tile = Framing.GetTileSafely(pos.X, pos.Y);
            if (tile.type != ModContent.TileType<SpawnBlock>() && tile.active())
            {
                HitTargetPos(pos);
            }
            npc.direction = 1;
        }

        private void HitTargetPos(Point pos)
        {
            SetDamage(pos, Framing.GetTileSafely(pos.X, pos.Y));
            if (hitTile.AddDamage(tileId, damage) >= 100)
            {
                hitTile.Clear(tileId);
                WorldGen.KillTile(pos.X, pos.Y);
                damage = 0;
            }
        }

        private void MineBlockBottomRight()
        {
            var pos = npc.BottomRight.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            Tile tile = Framing.GetTileSafely(pos.X, pos.Y);
            if (tile.type != ModContent.TileType<SpawnBlock>() && tile.active())
            {
                HitTargetPos(pos);
            }
            npc.direction = 1;
        }

        private void SetDamage(Point pos, Tile tile)
        {
            tileId = hitTile.HitObject(pos.X, pos.Y, 1);
            if (Main.tileHammer[tile.type])
            {
                TileLoader.MineDamage(hammer.hammer, ref damage);
            }
            else if (Main.tileAxe[tile.type])
            {
                TileLoader.MineDamage(axe.axe, ref damage);
            }
            else
            {
                TileLoader.MineDamage(pickaxe.pick, ref damage);
            }
        }

        private void MineBlockBottomLeft()
        {
            var pos = npc.BottomLeft.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var tile = Framing.GetTileSafely(pos.X, pos.Y);
            if (tile.type != ModContent.TileType<SpawnBlock>() && tile.active())
                HitTargetPos(pos);
            npc.direction = -1;
        }

        private void MineBlockBottom()
        {
            var pos = npc.Bottom.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockTopRight(int x, int y)
        {
            var pos = npc.TopRight.ToTileCoordinates();
            pos.X += x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
            npc.direction = 1;
        }

        private void MineBlockTopLeft(int x, int y)
        {
            var pos = npc.TopLeft.ToTileCoordinates();
            pos.X -= x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
            npc.direction = -1;
        }

        private void MineBlockTop(int y)
        {
            var pos = npc.Top.ToTileCoordinates();
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
        }

        private void DoActions(int action, int direction, string itemToCraft, string blockToPlace, int x, int y)
        {
            switch (action)
            {
                case (int)OutputType.Jump:
                    Jump();
                    break;
                case (int)OutputType.MineBlock:
                    MineBlock(direction, x, y);
                    break;
                case (int)OutputType.Move:
                    Move(direction);
                    break;
                case (int)OutputType.PlaceBlock:
                    PlaceBlock(direction, blockToPlace, x, y);
                    break;
                case (int)OutputType.CraftItem:
                    CraftItem(itemToCraft);
                    break;
                default:
                    break;
            }
        }

        private void CraftItem(string itemToCraft)
        {
            bool canCraft = false;
            int availableIngredientCount = 0;
            int tileCount = 0;
            RecipeFinder finder = new RecipeFinder();
            ItemID.Search.TryGetId(itemToCraft.Replace(" ", ""), out int id);
            finder.SetResult(id);
            List<Recipe> recipes = finder.SearchRecipes();
            if (recipes != null && inventory != null)
            {
                int requiredIngredientCount = 0;
                int requiredTileCount = 0;
                foreach (Item ingredient in recipes[0].requiredItem)
                {
                    if (ingredient.active)
                    {
                        requiredIngredientCount++;
                        foreach (Item inventoryItem in inventory)
                        {
                            if (inventoryItem.Name == ingredient.Name && inventoryItem.stack >= ingredient.stack)
                            {
                                availableIngredientCount++;
                            }
                        }
                    }
                }
                foreach (int tileId in recipes[0].requiredTile)
                {
                    if (tileId != -1)
                    {
                        requiredTileCount++;
                        if (IsTileNearby(tileId))
                            tileCount++;
                    }
                }

                if (requiredTileCount == tileCount && requiredIngredientCount == availableIngredientCount)
                    canCraft = true;
            }
            if (canCraft)
            {
                if (inventory != null)
                {
                    var item = inventory.Find(x => x.Name == itemToCraft && x.active);
                    if (item != null)
                    {
                        item.stack += recipes[0].createItem.stack;
                    }
                    else
                    {
                        inventory.Add(new Item());
                        int itemId = ItemID.Search.GetId(recipes[0].createItem.Name.Replace(" ", ""));
                        inventory[lastItemIndex].SetDefaults(itemId);
                        inventory[lastItemIndex].stack = recipes[0].createItem.stack;
                        lastItemIndex++;
                    }

                    foreach (Item invItem in inventory)
                    {
                        foreach (Item reqItem in recipes[0].requiredItem)
                        {
                            if (invItem.Name == reqItem.Name && invItem.stack > 0)
                            {
                                invItem.stack -= reqItem.stack;
                            }
                        }
                    }
                }
            }
        }

        private bool IsTileNearby(int tileId)
        {
            for (int x = -5; x <= 5; x++)
            {
                for (int y = -5; y <= 5; y++)
                {
                    if (Framing.GetTileSafely((int)npc.Center.ToTileCoordinates().X + x, (int)npc.Center.ToTileCoordinates().Y + y).type == tileId)
                        return true;
                }
            }
            return false;
        }

        private void PlaceBlock(int direction, string blockToPlace, int x, int y)
        {
            switch (direction)
            {
                case (int)Direction.Bottom:
                    PlaceBlockBottom(blockToPlace, y);
                    break;
                case (int)Direction.BottomLeft:
                    PlaceBlockBottomLeft(blockToPlace, x, y);
                    break;
                case (int)Direction.BottomRight:
                    PlaceBlockBottomRight(blockToPlace, x, y);
                    break;
                case (int)Direction.Top:
                    PlaceBlockTop(blockToPlace, y);
                    break;
                case (int)Direction.TopLeft:
                    PlaceBlockTopLeft(blockToPlace, x, y);
                    break;
                case (int)Direction.TopRight:
                    PlaceBlockTopRight(blockToPlace, x, y);
                    break;
                case (int)Direction.Left:
                    PlaceBlockLeft(blockToPlace, x);
                    break;
                case (int)Direction.Right:
                    PlaceBlockRight(blockToPlace, x);
                    break;
            }
        }

        private void Move(int direction)
        {
            switch (direction)
            {
                case (int)MoveDirection.Left:
                    MoveLeft();
                    break;
                case (int)MoveDirection.Right:
                    MoveRight();
                    break;
            }
        }

        private void MineBlock(int direction, int x, int y)
        {
            switch (direction)
            {
                case (int)Direction.Bottom:
                    MineBlockBottom();
                    break;
                case (int)Direction.BottomLeft:
                    MineBlockBottomLeft();
                    break;
                case (int)Direction.BottomRight:
                    MineBlockBottomRight();
                    break;
                case (int)Direction.Top:
                    MineBlockTop(y);
                    break;
                case (int)Direction.TopLeft:
                    MineBlockTopLeft(x, y);
                    break;
                case (int)Direction.TopRight:
                    MineBlockTopRight(x, y);
                    break;
                case (int)Direction.Left:
                    MineBlockLeft(x);
                    break;
                case (int)Direction.Right:
                    MineBlockRight(x);
                    break;
            }
        }

        public override void DrawEffects(ref Color drawColor)
        {
            if (organism != null)
                drawColor = drawColor.MultiplyRGB(Color.Green);
        }

        public override bool CanChat()
        {
            return true;
        }

        public override string GetChat()
        {
            string inventoryItems = "";
            if (inventory != null)
            {
                foreach (Item item in inventory)
                {
                    inventoryItems += "\n" + item.Name + " x" + item.stack;
                }
            }
            return "Inventory: " + inventoryItems;
        }
    }
}

