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

namespace ChaosTerraria.NPCs
{
    class AdamZero : ModNPC
    {
        public override string Texture => "ChaosTerraria/NPCs/Terrarian";
        private static int timer;
        private int timeLeft = 0;
        private int[] tiles = new int[25];
        internal Organism organism;
        private int lifeTicks = 600;
        private Item[] items = new Item[40];
        internal SpawnBlockTileEntity spawnBlockTileEntity;

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
        }

        public override void AI()
        {
            timer++;
            timeLeft++;

            if (timer > 18 && npc.active == true)
            {
                if (organism != null && tiles != null)
                {
                    int action = organism.nNet.GetOutput(npc.Center, "AdamZero", out int direction);
                    DoActions(action, direction);
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

        public void DoActions(int action, int direction)
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
                default:
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
            if (organism != null)
                return organism.nameSpace + "\n" + "Role Name: " + organism.trainingRoomRoleNamespace;
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

