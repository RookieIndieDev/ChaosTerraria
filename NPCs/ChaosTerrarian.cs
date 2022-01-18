
using ChaosTerraria.Classes;
using ChaosTerraria.Enums;
using ChaosTerraria.Fitness;
using ChaosTerraria.Managers;
using ChaosTerraria.Structs;
using ChaosTerraria.TileEntities;
using ChaosTerraria.Tiles;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosTerraria.NPCs
{
    //TODO: Display Score in NPC chat window
    //TODO: Change Sprite
    //TODO: Implement fitness event in NPC Chat window?
    public class ChaosTerrarian : ModNPC
    {
        public override string Texture => "ChaosTerraria/NPCs/Terrarian";

        private int actionTimer;
        private int lifeTimer = 0;
        private int currentAction = -1;
        internal Organism organism;
        private bool orgAssigned = false;
        private Report report;
        private int lifeTicks;
        public List<Item> inventory = new List<Item>();
        Tile lastPlacedTile;
        Tile lastMinedTile;
        int lastMinedTileType = -1;
        string craftedItem;
        internal SpawnBlockTileEntity spawnBlockTileEntity;
        FitnessManager fitnessManager;
        private int inventoryLastItemIndex = 0;
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
            npc.aiStyle = -1;
            npc.townNPC = false;
            npc.friendly = true;
            npc.width = 18;
            npc.height = 40;
            npc.damage = 10;
            npc.defense = 10;
            npc.lifeMax = 100;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.Guide;
            npc.homeless = true;
            npc.noGravity = false;
            npc.dontTakeDamage = false;
        }

        public override bool CheckDead()
        {
            SpawnManager.activeBotCount--;
            SpawnManager.totalSpawned++;
            lifeTimer = 0;
            npc.life = 0;
            if (spawnBlockTileEntity != null)
                spawnBlockTileEntity.spawnedSoFar--;
            if (SessionManager.ObservableNPCs != null && SessionManager.ObservableNPCs.Count > 0)
                SessionManager.ObservableNPCs.Remove(this);
            return true;
        }

        public override void AI()
        {
            int lifeEffect = 0;
            if (SessionManager.Organisms != null && !orgAssigned && SessionManager.Organisms.Count > 0)
            {
                organism = SessionManager.GetOrganism();
                if (organism != null)
                {
                    npc.GivenName = organism.nameSpace;
                    report.nameSpace = organism.nameSpace;
                }
                orgAssigned = true;
                foreach (Role role in SessionManager.Package.roles)
                {
                    if (role.nameSpace == organism.trainingRoomRoleNamespace)
                    {
                        foreach (Setting setting in role.settings)
                        {
                            switch (setting.nameSpace)
                            {
                                case "BASE_LIFE_SECONDS":
                                    lifeTicks = int.Parse(setting.value) * 60;
                                    break;
                                case "INV_1":
                                    AddItemToInventory(setting);
                                    break;
                                case "INV_2":
                                    AddItemToInventory(setting);
                                    break;
                                case "friendly":
                                    npc.friendly = bool.Parse(setting.value);
                                    break;
                            }
                        }
                        fitnessManager = new FitnessManager(JsonConvert.DeserializeObject<List<FitnessRule>>(role.fitnessRulesRaw));
                        break;
                    }
                }
            }

            actionTimer++;
            lifeTimer++;
            if (actionTimer > 18 && npc.active)
            {
                if (organism != null)
                {
                    int action = organism.nNet.GetOutput(npc.Center.ToTileCoordinates(), inventory, out int direction, out string itemToCraft, out string itemToPlace, out int x, out int y);
                    currentAction = action;
                    DoActions(action, direction, itemToCraft, itemToPlace, x, y);
                    UpdateInventory();
                }

                if (SessionManager.Package.roles != null && fitnessManager != null)
                    report.score += fitnessManager.TestFitness(this, lastMinedTileType, lastPlacedTile, craftedItem, out lifeEffect);
                lastMinedTile = null;
                lastPlacedTile = null;
                lastMinedTileType = -1;
                craftedItem = "";
                actionTimer = 0;
                lifeTicks += (lifeEffect * 60);
            }

            if (lifeTimer > lifeTicks && npc.active)
            {
                RecipeFinder recipeFinder = new RecipeFinder();
                foreach (Item item in inventory)
                {
                    var name = item.Name.Replace(" ", "");
                    int id = ItemID.Search.GetId(name);
                    recipeFinder.AddIngredient(id);
                    List<Recipe> recipes = recipeFinder.SearchRecipes();
                    if (recipes != null)
                    {
                        switch (item.Name)
                        {
                            case "Wood":
                                AddMatchingRecipe(recipes, "Wooden");
                                break;
                            case "RichMahogony":
                                AddMatchingRecipe(recipes, item.Name);
                                break;
                            case "Ebonwood":
                                AddMatchingRecipe(recipes, item.Name);
                                break;
                            case "Shadewood":
                                AddMatchingRecipe(recipes, item.Name);
                                break;
                            case "Pearlwood":
                                AddMatchingRecipe(recipes, item.Name);
                                break;
                            case "BorealWood":
                                AddMatchingRecipe(recipes, item.Name);
                                break;
                            case "PalmWood":
                                AddMatchingRecipe(recipes, item.Name);
                                break;
                            case "DynastyWood":
                                AddMatchingRecipe(recipes, item.Name);
                                break;
                            case "SpookyWood":
                                AddMatchingRecipe(recipes, item.Name);
                                break;                            
                            default:
                                foreach (Recipe recipe in recipes)
                                {
                                    AddRecipeObsAttr(recipe);
                                }
                                break;
                        }


                    }
                    ObservedAttributes inventoryObsAttr;
                    inventoryObsAttr.attributeId = "ITEM_ID";
                    inventoryObsAttr.attributeValue = item.Name;
                    inventoryObsAttr.species = organism.speciesNamespace;
                    if (!SessionManager.ObservedAttributes.Contains(inventoryObsAttr))
                        SessionManager.ObservedAttributes.Add(inventoryObsAttr);
                }

                if (organism != null && !SessionManager.Reports.Contains(report))
                {
                    SessionManager.Reports.Add(report);
                }

                SpawnManager.activeBotCount--;
                SpawnManager.totalSpawned++;
                lifeTimer = 0;
                npc.life = 0;
                if (spawnBlockTileEntity != null)
                    spawnBlockTileEntity.spawnedSoFar--;
                if (SessionManager.ObservableNPCs != null && SessionManager.ObservableNPCs.Count > 0)
                    SessionManager.ObservableNPCs.Remove(this);
            }
        }

        private void AddItemToInventory(Setting setting)
        {
            if (setting.value != "none")
            {
                var settingValue = setting.value.Split('@');
                inventory.Add(new Item());
                inventory[inventoryLastItemIndex].SetDefaults(ItemID.TypeFromUniqueKey("Terraria " + settingValue[0]));
                inventory[inventoryLastItemIndex].stack = int.Parse(settingValue[1]);
                inventoryLastItemIndex++;
            }
        }

        private void AddMatchingRecipe(List<Recipe> recipes, string startString)
        {
            foreach (Recipe recipe in recipes)
            {
                if (recipe.createItem.Name.StartsWith(startString))
                    AddRecipeObsAttr(recipe);
            }
        }

        private void AddRecipeObsAttr(Recipe recipe)
        {
            ObservedAttributes observedAttr;
            observedAttr.attributeId = "RECIPE_ID";
            observedAttr.attributeValue = recipe.createItem.Name;
            observedAttr.species = organism.speciesNamespace;
            if (!SessionManager.ObservedAttributes.Contains(observedAttr))
                SessionManager.ObservedAttributes.Add(observedAttr);
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
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).active() && (Framing.GetTileSafely(pos.X, pos.Y + 1).active() || Framing.GetTileSafely(pos.X, pos.Y - 1).active()))
            {
                if (blockToPlace.Contains("Door") && item.createTile != -1)
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    item.stack--;
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                }
                else if (item.createTile != -1)
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                    item.stack--;
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                }
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
                if (blockToPlace.Contains("Door") && item.createTile != -1)
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    item.stack--;
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                }
                else if (item.createTile != -1)
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                    item.stack--;
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                }
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
                if (blockToPlace.Contains("Door") && item.createTile != -1)
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                    item.stack--;
                }
                else if (item.createTile != -1)
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                    item.stack--;
                }
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
                if (blockToPlace.Contains("Door") && item.createTile != -1)
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                    item.stack--;
                }
                else if (item.createTile != -1)
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                    item.stack--;
                }
            }
            npc.direction = -1;
        }

        private void PlaceBlockBottom(string blockToPlace, int y)
        {
            var pos = npc.Bottom.ToTileCoordinates();
            pos.Y += y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).active() && (Framing.GetTileSafely(pos.X, pos.Y + 1).active() || Framing.GetTileSafely(pos.X, pos.Y - 1).active()))
            {
                if (blockToPlace.Contains("Door") && item.createTile != -1)
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                    item.stack--;
                }
                else if (item.createTile != -1)
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                    item.stack--;
                }
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
                if (blockToPlace.Contains("Door") && item.createTile != -1)
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    item.stack--;
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                }
                else if (item.createTile != -1)
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                    item.stack--;
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                }
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
                if (blockToPlace.Contains("Door") && item.createTile != -1)
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                    item.stack--;
                }
                else if (item.createTile != -1)
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                    item.stack--;
                }
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
                if (blockToPlace.Contains("Door") && item.createTile != -1)
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    item.stack--;
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                }
                else if (Framing.GetTileSafely(pos.X - 1, pos.Y).active() || Framing.GetTileSafely(pos.X + 1, pos.Y).active() || Framing.GetTileSafely(pos.X, pos.Y + 1).active())
                {
                    if (item.createTile != -1)
                    {
                        WorldGen.PlaceTile(pos.X, pos.Y, item.createTile);
                        item.stack--;
                        lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                    }
                }
            }
        }

        private void MineBlockLeft(int range)
        {
            var pos = npc.Left.ToTileCoordinates();
            pos.X -= range;
            pos.Y++;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.active())
            {
                WorldGen.KillTile(pos.X, pos.Y);
                npc.direction = -1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockRight(int range)
        {
            var pos = npc.Right.ToTileCoordinates();
            pos.X += range;
            pos.Y++;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.active())
            {
                WorldGen.KillTile(pos.X, pos.Y);
                npc.direction = 1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockBottomRight()
        {
            var pos = npc.BottomRight.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.active())
            {
                WorldGen.KillTile(pos.X, pos.Y);
                npc.direction = 1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockBottomLeft()
        {
            var pos = npc.BottomLeft.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.active())
            {
                WorldGen.KillTile(pos.X, pos.Y);
                npc.direction = -1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockBottom()
        {
            var pos = npc.Bottom.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.active())
            {
                WorldGen.KillTile(pos.X, pos.Y);
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockTopRight(int x, int y)
        {
            var pos = npc.TopRight.ToTileCoordinates();
            pos.X += x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.active())
            {
                WorldGen.KillTile(pos.X, pos.Y);
                npc.direction = 1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockTopLeft(int x, int y)
        {
            var pos = npc.TopLeft.ToTileCoordinates();
            pos.X -= x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.active())
            {
                WorldGen.KillTile(pos.X, pos.Y);
                npc.direction = -1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockTop(int y)
        {
            var pos = npc.Top.ToTileCoordinates();
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.White, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.active())
            {
                WorldGen.KillTile(pos.X, pos.Y);
            }
            else
            {
                lastMinedTile = null;
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

        private void CraftItem(string itemToCraft)
        {
            bool canCraft = false;
            int availableIngredientCount = 0;
            int tileCount = 0;
            List<Recipe> recipes = null;
            RecipeFinder finder = new RecipeFinder();
            itemToCraft = itemToCraft == "Wooden Yoyo" ? "WoodYoyo" : itemToCraft.Replace(" ", "");
            ItemID.Search.TryGetId(itemToCraft, out int id);
            if (id != 0)
            {
                finder.SetResult(id);
                recipes = finder.SearchRecipes();
            }

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
                        int itemId = ItemID.Search.GetId(itemToCraft);
                        inventory[inventoryLastItemIndex].SetDefaults(itemId);
                        inventory[inventoryLastItemIndex].stack = recipes[0].createItem.stack;
                        inventoryLastItemIndex++;
                    }
                    craftedItem = itemToCraft;
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

        private void DoActions(int action, int direction, string itemToCraft, string blockToPlace, int x, int y)
        {
            switch (action)
            {
                case (int)OutputType.Jump:
                    Jump();
                    break;
                case (int)OutputType.Move:
                    Move(direction);
                    break;
                case (int)OutputType.MineBlock:
                    MineBlock(direction, x, y);
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

        private void UpdateInventory()
        {
            if (inventory != null)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i].stack == 0)
                    {
                        inventory.Remove(inventory[i]);
                        inventoryLastItemIndex--;
                    }
                }
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

            if (organism != null)
                return organism.nameSpace + "\n" + "Role Name: " + organism.trainingRoomRoleNamespace
                    + "\n" + "Current Action: " + (OutputType)currentAction + "\n" + "Time Left: " + ((lifeTicks - lifeTimer) / 60) + "\nInventory: " + inventoryItems + "\nScore: " + report.score;
            return "Org Not Assigned";
        }
    }
}


