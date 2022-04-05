using ChaosTerraria.AI;
using ChaosTerraria.Classes;
using ChaosTerraria.Enums;
using ChaosTerraria.Managers;
using ChaosTerraria.Structs;
using ChaosTerraria.TileEntities;
using ChaosTerraria.Tiles;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ChaosTerraria.NPCs
{
    //TODO: Add realistic block breaking
    //No of ticks = time in seconds * 60
    public class AdamZero : ModNPC
    {
        public override string Texture => "ChaosTerraria/NPCs/Terrarian";

        private const int ticksUntilNextAction = 18;
        private int timer;
        private int timeLeft;
        internal Organism organism;
        private Building houseBlocksFromFile;
        private int lifeTicks = 600;
        private List<Item> inventory = new List<Item>();
        internal SpawnBlockTileEntity spawnBlockTileEntity;
        int lastItemIndex;
        HitTile hitTile = new();
        Item axe = new();
        Item pickaxe = new();
        Item hammer = new();
        private int damage;
        private int tileId;
        private int correctBlocks;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.MustAlwaysDraw[Type] = true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            SpawnManager.adamZeroCount--;
            timeLeft = 0;
            NPC.life = 0;
            spawnBlockTileEntity.spawnedSoFar--;
            if (SessionManager.ObservableNPCs != null && SessionManager.ObservableNPCs.Count > 0)
                SessionManager.ObservableNPCs.Remove(this);
            return true;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.townNPC = false;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 100;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
            NPC.homeless = true;
            NPC.noGravity = false;
            NPC.dontTakeDamage = false;
#if DEBUG
            organism = new Organism
            {
                nNet = JsonConvert.DeserializeObject<NNet>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                + @"\My Games\Terraria\tModLoader\ModSources\ChaosTerraria\NNet.json")),
                nameSpace = "AdamZero",
                trainingRoomRoleNamespace = "AdamZero"
            };
            houseBlocksFromFile = JsonConvert.DeserializeObject<Building>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            + @"\My Games\Terraria\tModLoader\ModSources\ChaosTerraria\House.json"));
#endif
            NPC.GivenName = "AdamZero";
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
            spawnBlockTileEntity.spawnedSoFar--;
            if (SessionManager.ObservableNPCs != null && SessionManager.ObservableNPCs.Count > 0)
                SessionManager.ObservableNPCs.Remove(this);
            if (timer > ticksUntilNextAction && NPC.active)
            {
                if (organism != null)
                {
                    int action = organism.nNet.GetOutput(NPC.Center.ToTileCoordinates(), inventory, out int direction, out string itemToCraft, out string blockToPlace, out int x, out int y);
                    DoActions(action, direction, itemToCraft, blockToPlace, x, y);
                    UpdateInventory();
                }
                timer = 0;
            }
            if (timeLeft > lifeTicks && NPC.active)
            {
                SpawnManager.adamZeroCount--;
                timeLeft = 0;
                NPC.life = 0;
                //TODO: Move into FitnessManager after some cleanup
                foreach (BuildingBlock houseBlock in houseBlocksFromFile.houseBlocks)
                {
                    Vector2 pos = new((float)(spawnBlockTileEntity.Position.X + houseBlock.x) * 16, (float)(spawnBlockTileEntity.Position.Y - houseBlock.y) * 16);
                    Tile tile = Framing.GetTileSafely(pos);
                    Dust.QuickBox(pos, new Vector2(pos.X + 16, pos.Y + 16), 2, houseBlock.wall ? Color.Teal : Color.White, null);
                    if (!houseBlock.wall && tile.TileType == TileID.Search.GetId(houseBlock.type))
                    {
                        correctBlocks++;
                    }

                    if (houseBlock.wall && Framing.GetTileSafely(pos).WallType == WallID.Search.GetId(houseBlock.type))
                    {
                        correctBlocks++;
                    }
                }
                Main.NewText($"Number of correct blocks placed: {correctBlocks}", Color.Green);
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
            NPC.velocity.X = 0.5f;
            NPC.direction = 1;
        }

        private void MoveLeft()
        {
            NPC.velocity.X = -0.5f;
            NPC.direction = -1;
        }

        private void Jump()
        {
            if (NPC.collideY)
            {
                NPC.velocity.Y = -8f;
            }
        }

        private void PlaceBlockLeft(string blockToPlace, int x)
        {
            var pos = NPC.Left.ToTileCoordinates();
            pos.X -= x;
            pos.Y = blockToPlace.Contains("Door") ? pos.Y : pos.Y + 1;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).HasTile && (Framing.GetTileSafely(pos.X, pos.Y + 1).HasTile || Framing.GetTileSafely(pos.X, pos.Y - 1).HasTile))
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

            NPC.direction = -1;
        }

        private void PlaceBlockRight(string blockToPlace, int x)
        {
            var pos = NPC.Right.ToTileCoordinates();
            pos.X += x;
            pos.Y = blockToPlace.Contains("Door") ? pos.Y : pos.Y + 1;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).HasTile && (Framing.GetTileSafely(pos.X, pos.Y + 1).HasTile || Framing.GetTileSafely(pos.X, pos.Y - 1).HasTile))
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

            NPC.direction = 1;
        }

        private void PlaceBlockBottomRight(string blockToPlace, int x, int y)
        {
            var pos = NPC.BottomRight.ToTileCoordinates();
            pos.X += x;
            pos.Y += y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).HasTile && (Framing.GetTileSafely(pos.X, pos.Y + 1).HasTile || Framing.GetTileSafely(pos.X, pos.Y - 1).HasTile))
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
            NPC.direction = 1;
        }

        private void PlaceBlockBottomLeft(string blockToPlace, int x, int y)
        {
            var pos = NPC.BottomLeft.ToTileCoordinates();
            pos.X -= x;
            pos.Y += y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).HasTile && (Framing.GetTileSafely(pos.X, pos.Y + 1).HasTile || Framing.GetTileSafely(pos.X, pos.Y - 1).HasTile))
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

            NPC.direction = -1;
        }

        private void PlaceBlockBottom(string blockToPlace, int y)
        {
            var pos = NPC.Bottom.ToTileCoordinates();
            pos.Y += y;
            var item = FindInventoryItemStack(blockToPlace);
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).HasTile && (Framing.GetTileSafely(pos.X, pos.Y + 1).HasTile || Framing.GetTileSafely(pos.X, pos.Y - 1).HasTile))
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
            var pos = NPC.TopRight.ToTileCoordinates();
            pos.X += x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).HasTile && (Framing.GetTileSafely(pos.X, pos.Y + 1).HasTile || Framing.GetTileSafely(pos.X, pos.Y - 1).HasTile))
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

            NPC.direction = 1;
        }

        private void PlaceBlockTopLeft(string blockToPlace, int x, int y)
        {
            var pos = NPC.TopLeft.ToTileCoordinates();
            pos.X -= x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).HasTile && (Framing.GetTileSafely(pos.X, pos.Y + 1).HasTile || Framing.GetTileSafely(pos.X, pos.Y - 1).HasTile))
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

            NPC.direction = -1;
        }

        private void PlaceBlockTop(string blockToPlace, int y)
        {
            var pos = NPC.Top.ToTileCoordinates();
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).HasTile && (Framing.GetTileSafely(pos.X, pos.Y + 1).HasTile || Framing.GetTileSafely(pos.X, pos.Y - 1).HasTile))
            {
                if (blockToPlace.Contains("Door"))
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    item.stack--;
                }
                else if (Framing.GetTileSafely(pos.X - 1, pos.Y).HasTile || Framing.GetTileSafely(pos.X + 1, pos.Y).HasTile || Framing.GetTileSafely(pos.X, pos.Y + 1).HasTile)
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                    item.stack--;
                }
            }
        }

        private void MineBlockLeft(int range)
        {
            var pos = NPC.Left.ToTileCoordinates();
            pos.X -= range;
            pos.Y++;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            var tile = Framing.GetTileSafely(pos.X, pos.Y);
            if (tile.TileType != ModContent.TileType<SpawnBlock>() && tile.HasTile)
            {
                HitTargetPos(pos);
            }
            NPC.direction = -1;
        }

        private void MineBlockRight(int range)
        {
            var pos = NPC.Right.ToTileCoordinates();
            pos.X += range;
            pos.Y++;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            var tile = Framing.GetTileSafely(pos.X, pos.Y);
            if (tile.TileType != ModContent.TileType<SpawnBlock>() && tile.HasTile)
            {
                HitTargetPos(pos);
            }
            NPC.direction = 1;
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
            var pos = NPC.BottomRight.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            Tile tile = Framing.GetTileSafely(pos.X, pos.Y);
            if (tile.TileType != ModContent.TileType<SpawnBlock>() && tile.HasTile)
            {
                HitTargetPos(pos);
            }
            NPC.direction = 1;
        }

        private void SetDamage(Point pos, Tile tile)
        {
            tileId = hitTile.HitObject(pos.X, pos.Y, 1);
            if (Main.tileHammer[tile.TileType])
            {
                TileLoader.MineDamage(hammer.hammer, ref damage);
            }
            else if (Main.tileAxe[tile.TileType])
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
            var pos = NPC.BottomLeft.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            var tile = Framing.GetTileSafely(pos.X, pos.Y);
            if (tile.TileType != ModContent.TileType<SpawnBlock>() && tile.HasTile)
                HitTargetPos(pos);
            NPC.direction = -1;
        }

        private void MineBlockBottom()
        {
            var pos = NPC.Bottom.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            if (Framing.GetTileSafely(pos.X, pos.Y).TileType != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockTopRight(int x, int y)
        {
            var pos = NPC.TopRight.ToTileCoordinates();
            pos.X += x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            if (Framing.GetTileSafely(pos.X, pos.Y).TileType != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
            NPC.direction = 1;
        }

        private void MineBlockTopLeft(int x, int y)
        {
            var pos = NPC.TopLeft.ToTileCoordinates();
            pos.X -= x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            if (Framing.GetTileSafely(pos.X, pos.Y).TileType != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
            NPC.direction = -1;
        }

        private void MineBlockTop(int y)
        {
            var pos = NPC.Top.ToTileCoordinates();
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            if (Framing.GetTileSafely(pos.X, pos.Y).TileType != ModContent.TileType<SpawnBlock>())
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
            Recipe requiredRecipe = null;
            int availableIngredientCount = 0;
            int tileCount = 0;
            ItemID.Search.TryGetId(itemToCraft.Replace(" ", ""), out int id);
            foreach (Recipe recipe in Main.recipe)
            {
                if (recipe.createItem.type == id)
                {
                    requiredRecipe = recipe;
                    break;
                }
            }
            if (requiredRecipe != null && inventory != null)
            {
                int requiredIngredientCount = 0;
                int requiredTileCount = 0;
                foreach (Item ingredient in requiredRecipe.requiredItem)
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
                foreach (int tileId in requiredRecipe.requiredTile)
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
                        item.stack += requiredRecipe.createItem.stack;
                    }
                    else
                    {
                        inventory.Add(new Item());
                        int itemId = ItemID.Search.GetId(requiredRecipe.createItem.Name.Replace(" ", ""));
                        inventory[lastItemIndex].SetDefaults(itemId);
                        inventory[lastItemIndex].stack = requiredRecipe.createItem.stack;
                        lastItemIndex++;
                    }

                    foreach (Item invItem in inventory)
                    {
                        foreach (Item reqItem in requiredRecipe.requiredItem)
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
                    if (Framing.GetTileSafely((int)NPC.Center.ToTileCoordinates().X + x, (int)NPC.Center.ToTileCoordinates().Y + y).TileType == tileId)
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

        //public override string GetChat()
        //{
        //    string inventoryItems = "";
        //    if (inventory != null)
        //    {
        //        foreach (Item item in inventory)
        //        {
        //            inventoryItems += "\n" + item.Name + " x" + item.stack;
        //        }
        //    }
        //    return "Inventory: " + inventoryItems;
        //}

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            foreach (Item item in inventory)
            {
                shop.item[nextSlot++] = item;
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            shop = true;
        }

        //public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        //{
        //    (Vector2 weaponStart, float rotation) = NPC.GetSwingStats(150, 100, NPC.spriteDirection, Terraria.GameContent.TextureAssets.Item[1].Value.Width, Terraria.GameContent.TextureAssets.Item[1].Value.Height);
        //    spriteBatch.Draw(Terraria.GameContent.TextureAssets.Item[1].Value, weaponStart + (weaponStart - NPC.Center) - screenPos,
        // }
    }
}

