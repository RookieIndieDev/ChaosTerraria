
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
    //TODO: Change Sprite
    //TODO: Implement fitness event in NPC Chat window?
    public class ChaosTerrarian : ModNPC
    {
        public override string Texture => "ChaosTerraria/NPCs/Terrarian";

        private const int ticksUntilNextAction = 18;
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
        List<Recipe> recipesForObs;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.MustAlwaysDraw[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.townNPC = false;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.damage = 10;
            NPC.defense = 10;
            NPC.lifeMax = 100;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
            NPC.homeless = true;
            NPC.noGravity = false;
            NPC.dontTakeDamage = false;
            recipesForObs = new List<Recipe>();
        }

        public override bool CheckDead()
        {
            SpawnManager.activeBotCount--;
            SpawnManager.totalSpawned++;
            lifeTimer = 0;
            NPC.life = 0;
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
                    NPC.GivenName = organism.nameSpace;
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
                                    NPC.friendly = bool.Parse(setting.value);
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
            if (actionTimer > ticksUntilNextAction && NPC.active)
            {
                if (organism != null)
                {
                    int action = organism.nNet.GetOutput(NPC.Center.ToTileCoordinates(), inventory, out int direction, out string itemToCraft, out string itemToPlace, out int x, out int y);
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

            if (lifeTimer > lifeTicks && NPC.active)
            {
                foreach (Item item in inventory)
                {
                    var name = item.Name.Replace(" ", "");
                    int id = ItemID.Search.GetId(name);
                    foreach (Recipe recipe in Main.recipe)
                    {
                        if (recipe.HasIngredient(id))
                            recipesForObs.Add(recipe);
                    }
                    if (recipesForObs != null)
                    {
                        switch (item.Name)
                        {
                            case "Wood":
                                AddMatchingRecipe(recipesForObs, "Wooden");
                                break;
                            case "RichMahogony":
                                AddMatchingRecipe(recipesForObs, item.Name);
                                break;
                            case "Ebonwood":
                                AddMatchingRecipe(recipesForObs, item.Name);
                                break;
                            case "Shadewood":
                                AddMatchingRecipe(recipesForObs, item.Name);
                                break;
                            case "Pearlwood":
                                AddMatchingRecipe(recipesForObs, item.Name);
                                break;
                            case "BorealWood":
                                AddMatchingRecipe(recipesForObs, item.Name);
                                break;
                            case "PalmWood":
                                AddMatchingRecipe(recipesForObs, item.Name);
                                break;
                            case "DynastyWood":
                                AddMatchingRecipe(recipesForObs, item.Name);
                                break;
                            case "SpookyWood":
                                AddMatchingRecipe(recipesForObs, item.Name);
                                break;
                            default:
                                foreach (Recipe recipe in recipesForObs)
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
                NPC.life = 0;
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
                inventory[inventoryLastItemIndex].SetDefaults(ItemID.Search.GetId(settingValue[0]));
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
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).IsActive && (Framing.GetTileSafely(pos.X, pos.Y + 1).IsActive || Framing.GetTileSafely(pos.X, pos.Y - 1).IsActive))
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
            NPC.direction = -1;
        }

        private void PlaceBlockRight(string blockToPlace, int x)
        {
            var pos = NPC.Right.ToTileCoordinates();
            pos.X += x;
            pos.Y = blockToPlace.Contains("Door") ? pos.Y : pos.Y + 1;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).IsActive && (Framing.GetTileSafely(pos.X, pos.Y + 1).IsActive || Framing.GetTileSafely(pos.X, pos.Y - 1).IsActive))
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
            NPC.direction = 1;
        }

        private void PlaceBlockBottomRight(string blockToPlace, int x, int y)
        {
            var pos = NPC.BottomRight.ToTileCoordinates();
            pos.X += x;
            pos.Y += y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).IsActive && (Framing.GetTileSafely(pos.X, pos.Y + 1).IsActive || Framing.GetTileSafely(pos.X, pos.Y - 1).IsActive))
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
            NPC.direction = 1;
        }

        private void PlaceBlockBottomLeft(string blockToPlace, int x, int y)
        {
            var pos = NPC.BottomLeft.ToTileCoordinates();
            pos.X -= x;
            pos.Y += y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).IsActive && (Framing.GetTileSafely(pos.X, pos.Y + 1).IsActive || Framing.GetTileSafely(pos.X, pos.Y - 1).IsActive))
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
            NPC.direction = -1;
        }

        private void PlaceBlockBottom(string blockToPlace, int y)
        {
            var pos = NPC.Bottom.ToTileCoordinates();
            pos.Y += y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).IsActive && (Framing.GetTileSafely(pos.X, pos.Y + 1).IsActive || Framing.GetTileSafely(pos.X, pos.Y - 1).IsActive))
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
            var pos = NPC.TopRight.ToTileCoordinates();
            pos.X += x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).IsActive && (Framing.GetTileSafely(pos.X, pos.Y + 1).IsActive || Framing.GetTileSafely(pos.X, pos.Y - 1).IsActive))
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
            NPC.direction = 1;
        }

        private void PlaceBlockTopLeft(string blockToPlace, int x, int y)
        {
            var pos = NPC.TopLeft.ToTileCoordinates();
            pos.X -= x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).IsActive && (Framing.GetTileSafely(pos.X, pos.Y + 1).IsActive || Framing.GetTileSafely(pos.X, pos.Y - 1).IsActive))
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
            NPC.direction = -1;
        }

        private void PlaceBlockTop(string blockToPlace, int y)
        {
            var pos = NPC.Top.ToTileCoordinates();
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Blue, null);
            var item = FindInventoryItemStack(blockToPlace);
            if (item != null && !Framing.GetTileSafely(pos.X, pos.Y).IsActive && (Framing.GetTileSafely(pos.X, pos.Y + 1).IsActive || Framing.GetTileSafely(pos.X, pos.Y - 1).IsActive))
            {
                if (blockToPlace.Contains("Door") && item.createTile != -1)
                {
                    WorldGen.PlaceDoor(pos.X, pos.Y, item.createTile);
                    item.stack--;
                    lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
                }
                else if (Framing.GetTileSafely(pos.X - 1, pos.Y).IsActive || Framing.GetTileSafely(pos.X + 1, pos.Y).IsActive || Framing.GetTileSafely(pos.X, pos.Y + 1).IsActive)
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
            var pos = NPC.Left.ToTileCoordinates();
            pos.X -= range;
            pos.Y++;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.IsActive)
            {
                WorldGen.KillTile(pos.X, pos.Y);
                NPC.direction = -1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockRight(int range)
        {
            var pos = NPC.Right.ToTileCoordinates();
            pos.X += range;
            pos.Y++;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.IsActive)
            {
                WorldGen.KillTile(pos.X, pos.Y);
                NPC.direction = 1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockBottomRight()
        {
            var pos = NPC.BottomRight.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.IsActive)
            {
                WorldGen.KillTile(pos.X, pos.Y);
                NPC.direction = 1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockBottomLeft()
        {
            var pos = NPC.BottomLeft.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.IsActive)
            {
                WorldGen.KillTile(pos.X, pos.Y);
                NPC.direction = -1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockBottom()
        {
            var pos = NPC.Bottom.ToTileCoordinates();
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.IsActive)
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
            var pos = NPC.TopRight.ToTileCoordinates();
            pos.X += x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.IsActive)
            {
                WorldGen.KillTile(pos.X, pos.Y);
                NPC.direction = 1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockTopLeft(int x, int y)
        {
            var pos = NPC.TopLeft.ToTileCoordinates();
            pos.X -= x;
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.IsActive)
            {
                WorldGen.KillTile(pos.X, pos.Y);
                NPC.direction = -1;
            }
            else
            {
                lastMinedTile = null;
            }
        }

        private void MineBlockTop(int y)
        {
            var pos = NPC.Top.ToTileCoordinates();
            pos.Y -= y;
            Dust.QuickBox(new Vector2(pos.X, pos.Y) * 16, new Vector2(pos.X + 1, pos.Y + 1) * 16, 2, Color.Green, null);
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            lastMinedTileType = lastMinedTile.type;
            if (lastMinedTile.type != ModContent.TileType<SpawnBlock>() && lastMinedTile.IsActive)
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
            Recipe requiredRecipe = null;
            itemToCraft = itemToCraft == "Wooden Yoyo" ? "WoodYoyo" : itemToCraft.Replace(" ", "");
            if (ItemID.Search.TryGetId(itemToCraft, out int id))
            {
                foreach (Recipe recipe in Main.recipe)
                {
                    if (recipe.createItem.type == id)
                    {
                        requiredRecipe = recipe;
                        break;
                    }
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
                        int itemId = ItemID.Search.GetId(itemToCraft);
                        inventory[inventoryLastItemIndex].SetDefaults(itemId);
                        inventory[inventoryLastItemIndex].stack = requiredRecipe.createItem.stack;
                        inventoryLastItemIndex++;
                    }
                    craftedItem = itemToCraft;
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
                    if (Framing.GetTileSafely((int)NPC.Center.ToTileCoordinates().X + x, (int)NPC.Center.ToTileCoordinates().Y + y).type == tileId)
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


