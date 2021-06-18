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
            npc.dontTakeDamage = true;
            organism = new Organism
            {
                nNet = JsonConvert.DeserializeObject<NNet>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) 
                + @"\My Games\Terraria\ModLoader\Mod Sources\ChaosTerraria\NNet.json")),
                nameSpace = "AdamZero",
                trainingRoomRoleNamespace="AdamZero"
            };
        }

        public override void AI()
        {
            npc.GivenName = organism.nameSpace;
            timer++;
            timeLeft++;

            if (timer > 15 && npc.active == true)
            {
                DoScan();
                if (organism != null && tiles != null)
                {
                    DoActions(organism.nNet.GetOutput(tiles, "AdamZero"));
                }
            }

            if (timeLeft > lifeTicks && npc.active == true)
            {
                SpawnManager.adamZeroCount--;
                timeLeft = 0;
                npc.life = 0;
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
        }

        private void PlaceBlockRight()
        {
            var pos = npc.Right.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
        }

        private void PlaceBlockBottomRight()
        {
            var pos = npc.BottomRight.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
        }

        private void PlaceBlockBottomLeft()
        {
            var pos = npc.BottomLeft.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
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
        }

        private void PlaceBlockTopLeft()
        {
            var pos = npc.TopLeft.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
        }

        private void PlaceBlockTop()
        {
            var pos = npc.Top.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
        }

        private void MineBlockLeft()
        {
            var pos = npc.Left.ToTileCoordinates();
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockRight()
        {
            var pos = npc.Right.ToTileCoordinates();
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockBottomRight()
        {
            var pos = npc.BottomRight.ToTileCoordinates();
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockBottomLeft()
        {
            var pos = npc.BottomLeft.ToTileCoordinates();
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockBottom()
        {
            var pos = npc.Bottom.ToTileCoordinates();
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockTopRight()
        {
            var pos = npc.TopRight.ToTileCoordinates();
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockTopLeft()
        {
            var pos = npc.TopLeft.ToTileCoordinates();
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockTop()
        {
            var pos = npc.Top.ToTileCoordinates();
            WorldGen.KillTile(pos.X, pos.Y);
        }

        public void DoActions(int action)
        {
            switch (action)
            {
                case (int)OutputType.Jump:
                    Jump();
                    break;
                case (int)OutputType.MoveRight:
                    MoveRight();
                    break;
                case (int)OutputType.MoveLeft:
                    MoveLeft();
                    break;
                case (int)OutputType.PlaceBlockTop:
                    PlaceBlockTop();
                    break;
                case (int)OutputType.PlaceBlockTopLeft:
                    PlaceBlockTopLeft();
                    break;
                case (int)OutputType.PlaceBlockTopRight:
                    PlaceBlockTopRight();
                    break;
                case (int)OutputType.PlaceBlockBottom:
                    PlaceBlockBottom();
                    break;
                case (int)OutputType.PlaceBlockBottomLeft:
                    PlaceBlockBottomLeft();
                    break;
                case (int)OutputType.PlaceBlockBottomRight:
                    PlaceBlockBottomRight();
                    break;
                case (int)OutputType.PlaceBlockRight:
                    PlaceBlockRight();
                    break;
                case (int)OutputType.PlaceBlockLeft:
                    PlaceBlockLeft();
                    break;
                case (int)OutputType.MineBlockTop:
                    MineBlockTop();
                    break;
                case (int)OutputType.MineBlockTopLeft:
                    MineBlockTopLeft();
                    break;
                case (int)OutputType.MineBlockTopRight:
                    MineBlockTopRight();
                    break;
                case (int)OutputType.MineBlockBottom:
                    MineBlockBottom();
                    break;
                case (int)OutputType.MineBlockBottomLeft:
                    MineBlockBottomLeft();
                    break;
                case (int)OutputType.MineBlockBottomRight:
                    MineBlockBottomRight();
                    break;
                case (int)OutputType.MineBlockLeft:
                    MineBlockLeft();
                    break;
                case (int)OutputType.MineBlockRight:
                    MineBlockRight();
                    break;
                default:
                    Main.NewText("Invalid Action");
                    break;
            }
        }



        public override void DrawEffects(ref Color drawColor)
        {
            if (organism != null)
                drawColor = drawColor.MultiplyRGB(Color.Green);
        }

        private void DoScan()
        {
            int range = 2;
            int blockCount = 0;
            int tileType;
            Point startPoint = npc.Center.ToTileCoordinates();

            for (int i = startPoint.X - range; i < startPoint.X + range; i++)
            {
                for (int j = startPoint.Y - range; j < startPoint.Y + range; j++)
                {
                    if (i >= 0 && i < Main.maxTilesX && j >= 0 && j < Main.maxTilesY)
                    {
                        tileType = Framing.GetTileSafely(i, j).type == 0 ? 1 : Framing.GetTileSafely(i, j).type;
                        tiles[blockCount++] = tileType;
                    }
                }
            }
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

