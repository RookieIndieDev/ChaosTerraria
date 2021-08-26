using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using ChaosTerraria.Classes;
using Microsoft.Xna.Framework;
using ChaosTerraria.Managers;
using ChaosTerraria.AI;
using System.IO;
using Newtonsoft.Json;
using ChaosTerraria.Enums;
using ChaosTerraria.Tiles;
using ChaosTerraria.TileEntities;
using System.Collections.Generic;

namespace ChaosTerraria.NPCs
{
    class AdamZero : ModNPC
    {
        public override string Texture => "ChaosTerraria/NPCs/Terrarian";
        private static int timer;
        private int timeLeft = 0;
        internal Organism organism;
        private int lifeTicks = 600;
        private List<Item> inventory = new List<Item>();
        internal SpawnBlockTileEntity spawnBlockTileEntity;
        int lastItemIndex = 0;
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

        public override void SetDefaults()
        {
            int id = 0;
            npc.aiStyle = -1;
            npc.townNPC = false;
            npc.friendly = true;
            npc.width = 18;
            npc.height = 40;
            npc.damage = 10;
            npc.defense = 10;
            npc.lifeMax = 100;
            npc.knockBackResist = 0.8f;
            animationType = NPCID.Guide;
            npc.homeless = true;
            npc.noGravity = false;
            npc.dontTakeDamage = true;
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
            ItemID.Search.TryGetId("Wood", out id);
            inventory[lastItemIndex].SetDefaults(id);
            inventory[lastItemIndex].stack = 10;
            lastItemIndex++;
            inventory.Add(new Item());
            ItemID.Search.TryGetId("WorkBench", out id);
            inventory[lastItemIndex].SetDefaults(id);
            inventory[lastItemIndex].stack = 10;
            lastItemIndex++;
            inventory.Add(new Item());
            ItemID.Search.TryGetId("Glass", out id);
            inventory[lastItemIndex].SetDefaults(id);
            inventory[lastItemIndex].stack = 10;
            lastItemIndex++;
        }

        public override void AI()
        {
            timer++;
            timeLeft++;

            if (timer > 18 && npc.active == true)
            {
                if (organism != null)
                {
                    int action = organism.nNet.GetOutput(npc.Center, "AdamZero", inventory, out int direction, out string itemToCraft);
                    DoActions(action, direction, itemToCraft);
                    UpdateInventory();
                }
            }

            if (timeLeft > lifeTicks && npc.active == true)
            {
                SpawnManager.adamZeroCount--;
                timeLeft = 0;
                npc.life = 0;
                spawnBlockTileEntity.spawnedSoFar--;
            }
        }

        private void UpdateInventory()
        {
            if (inventory != null)
            {
                for(int i = 0; i < inventory.Count; i++)
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

        private void PlaceBlockLeft()
        {
            var pos = npc.Left.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            npc.direction = -1;
        }

        private void PlaceBlockRight()
        {
            var pos = npc.Right.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            npc.direction = 1;
        }

        private void PlaceBlockBottomRight()
        {
            var pos = npc.BottomRight.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            npc.direction = 1;
        }

        private void PlaceBlockBottomLeft()
        {
            var pos = npc.BottomLeft.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            npc.direction = -1;
        }

        private void PlaceBlockBottom()
        {
            var pos = npc.Bottom.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
        }

        private void PlaceBlockTopRight()
        {
            var pos = npc.TopRight.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            npc.direction = 1;
        }

        private void PlaceBlockTopLeft()
        {
            var pos = npc.TopLeft.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            npc.direction = -1;
        }

        private void PlaceBlockTop()
        {
            var pos = npc.Top.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
        }

        private void MineBlockLeft()
        {
            var pos = npc.Left.ToTileCoordinates();
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
            npc.direction = -1;
        }

        private void MineBlockRight()
        {
            var pos = npc.Right.ToTileCoordinates();
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
            npc.direction = 1;
        }

        private void MineBlockBottomRight()
        {
            var pos = npc.BottomRight.ToTileCoordinates();
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
            npc.direction = 1;
        }

        private void MineBlockBottomLeft()
        {
            var pos = npc.BottomLeft.ToTileCoordinates();
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
            npc.direction = -1;
        }

        private void MineBlockBottom()
        {
            var pos = npc.Bottom.ToTileCoordinates();
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockTopRight()
        {
            var pos = npc.TopRight.ToTileCoordinates();
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
            npc.direction = 1;
        }

        private void MineBlockTopLeft()
        {
            var pos = npc.TopLeft.ToTileCoordinates();
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
            npc.direction = -1;
        }

        private void MineBlockTop()
        {
            var pos = npc.Top.ToTileCoordinates();
            if (Framing.GetTileSafely(pos.X, pos.Y).type != ModContent.TileType<SpawnBlock>())
                WorldGen.KillTile(pos.X, pos.Y);
        }

        public void DoActions(int action, int direction, string itemToCraft)
        {
            switch (action)
            {
                case (int)OutputType.Jump:
                    Jump();
                    break;
                case (int)OutputType.MineBlock:
                    MineBlock(direction);
                    break;
                case (int)OutputType.Move:
                    Move(direction);
                    break;
                case (int)OutputType.PlaceBlock:
                    PlaceBlock(direction);
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
            RecipeFinder finder = new RecipeFinder();
            ItemID.Search.TryGetId(itemToCraft.Replace(" ", ""), out int id);
            finder.SetResult(id);
            List<Recipe> recipes = finder.SearchRecipes();
            if (recipes != null && inventory != null)
            {
                int requiredIngredientCount = 0;
                
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

                if (requiredIngredientCount == availableIngredientCount)
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
                        int itemId = ItemID.Search.GetId(recipes[0].createItem.Name);
                        inventory[lastItemIndex].SetDefaults(itemId);
                        inventory[lastItemIndex].stack = recipes[0].createItem.stack;
                        lastItemIndex++;
                    }

                    foreach(Item invItem in inventory)
                    {
                        foreach(Item reqItem in recipes[0].requiredItem)
                        {
                            if(invItem.Name == reqItem.Name && invItem.stack > 0)
                            {
                                invItem.stack = invItem.stack - reqItem.stack;
                            }
                        }
                    }
                }
            }
        }

        private void PlaceBlock(int direction)
        {
            switch (direction)
            {
                case (int)Direction.Bottom:
                    PlaceBlockBottom();
                    break;
                case (int)Direction.BottomLeft:
                    PlaceBlockBottomLeft();
                    break;
                case (int)Direction.BottomRight:
                    PlaceBlockBottomRight();
                    break;
                case (int)Direction.Top:
                    PlaceBlockTop();
                    break;
                case (int)Direction.TopLeft:
                    PlaceBlockTopLeft();
                    break;
                case (int)Direction.TopRight:
                    PlaceBlockTopRight();
                    break;
                case (int)Direction.Left:
                    PlaceBlockLeft();
                    break;
                case (int)Direction.Right:
                    PlaceBlockRight();
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

        private void MineBlock(int direction)
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
                    MineBlockTop();
                    break;
                case (int)Direction.TopLeft:
                    MineBlockTopLeft();
                    break;
                case (int)Direction.TopRight:
                    MineBlockTopRight();
                    break;
                case (int)Direction.Left:
                    MineBlockLeft();
                    break;
                case (int)Direction.Right:
                    MineBlockRight();
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

