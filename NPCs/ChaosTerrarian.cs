using ChaosTerraria.Classes;
using ChaosTerraria.Enums;
using ChaosTerraria.Fitness;
using ChaosTerraria.Managers;
using ChaosTerraria.Structs;
using ChaosTerraria.TileEntities;
using ChaosTerraria.Tiles;
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

        private static int actionTimer;
        private int lifeTimer = 0;
        private int currentAction = -1;
        private int[] tiles = new int[25];
        internal Organism organism;
        private bool orgAssigned = false;
        private Report report;
        private int lifeTicks;
        private Item[] items = new Item[40];
        Tile lastPlacedTile;
        Tile lastMinedTile;
        int lastMinedTileType = -1;
        internal SpawnBlockTileEntity spawnBlockTileEntity;
        FitnessManager fitnessManager;

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
            npc.knockBackResist = 0.8f;
            animationType = NPCID.Guide;
            npc.homeless = true;
            npc.noGravity = false;
            npc.dontTakeDamage = true;
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
                foreach(Role role in SessionManager.Package.roles)
                {
                    if(role.nameSpace == organism.trainingRoomRoleNamespace)
                    {
                        foreach (Setting setting in role.settings)
                        {
                            if (setting.nameSpace == "BASE_LIFE_SECONDS")
                            {
                                lifeTicks = int.Parse(setting.value) * 60;
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
            if (actionTimer > 18 && npc.active == true)
            {
                if (organism != null && tiles != null)
                {
                    int action = organism.nNet.GetOutput(npc.Center, organism.speciesNamespace, out int direction);
                    currentAction = action;
                    DoActions(action, direction);
                }

                if (SessionManager.Package.roles != null && fitnessManager != null)
                    report.score += fitnessManager.TestFitness(this, lastMinedTileType, lastPlacedTile, out lifeEffect);
                lastMinedTile = null;
                lastPlacedTile = null;
                lastMinedTileType = -1;
                actionTimer = 0;
                lifeTicks += (lifeEffect * 60);
            }

            if (lifeTimer > lifeTicks && npc.active == true)
            {
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
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
            npc.direction = -1;
        }

        private void PlaceBlockRight()
        {
            var pos = npc.Right.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
            npc.direction = 1;
        }

        private void PlaceBlockBottomRight()
        {
            var pos = npc.BottomRight.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
            npc.direction = 1;
        }

        private void PlaceBlockBottomLeft()
        {
            var pos = npc.BottomLeft.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
            npc.direction = -1;
        }

        private void PlaceBlockBottom()
        {
            var pos = npc.Bottom.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
        }

        private void PlaceBlockTopRight()
        {
            var pos = npc.TopRight.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
            npc.direction = 1;
        }

        private void PlaceBlockTopLeft()
        {
            var pos = npc.TopLeft.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
            npc.direction = -1;
        }

        private void PlaceBlockTop()
        {
            var pos = npc.Top.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
        }

        private void MineBlockLeft()
        {
            var pos = npc.Left.ToTileCoordinates();
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

        private void MineBlockRight()
        {
            var pos = npc.Right.ToTileCoordinates();
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

        private void MineBlockTopRight()
        {
            var pos = npc.TopRight.ToTileCoordinates();
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

        private void MineBlockTopLeft()
        {
            var pos = npc.TopLeft.ToTileCoordinates();
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

        private void MineBlockTop()
        {
            var pos = npc.Top.ToTileCoordinates();
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

        public void DoActions(int action, int direction)
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
                    MineBlock(direction);
                    break;
                case (int)OutputType.PlaceBlock:
                    PlaceBlock(direction);
                    break;
                default:
                    break;
            }
        }

        public override bool CanChat()
        {
            return true;
        }

        public override string GetChat()
        {
            if (organism != null)
                return organism.nameSpace + "\n" + "Role Name: " + organism.trainingRoomRoleNamespace + "\n" + "Current Action: " + (OutputType) currentAction + "\n" + "Time Left: " + ((lifeTicks - lifeTimer)/60); 
            return "Org Not Assigned";
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ItemID.Wood);
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Bot Inventory";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
        }
    }
}


