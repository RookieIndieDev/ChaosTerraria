using System;
using ChaosTerraria.Classes;
using ChaosTerraria.Fitness;
using ChaosTerraria.Managers;
using ChaosTerraria.Structs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ChaosTerraria.NPCs
{
	//TODO: Display Score in NPC chat window
	//TODO: Change Sprite
	//TODO: Implement seconds to live and lifeEffect
	//TODO: Implement fitness event in NPC Chat window?
	public class ChaosTerrarian : ModNPC
	{
		public override string Texture => "ChaosTerraria/NPCs/Terrarian";

		private static int timer;
		private int timeLeft = 0;
		private int[] tiles = new int[25];
		internal Organism organism;
		private bool orgAssigned = false;
		private Report report;
		private int lifeTicks = 600;
        private Item[] items = new Item[40];
        Tile lastPlacedTile;
        Tile lastMinedTile;

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
		}

		public override void AI()
		{
			if (SessionManager.Organisms != null && !orgAssigned && SessionManager.Organisms.Count > 0)
			{
				organism = SessionManager.GetOrganism();
				if (organism != null)
				{
					npc.GivenName = organism.nameSpace;
					report.nameSpace = organism.nameSpace;
				}
				orgAssigned = true;
			}

			timer++;
			timeLeft++;

			if (timer > 15 && npc.active == true)
			{
				DoScan();
				if (organism != null && tiles != null)
				{
					DoActions(organism.nNet.GetOutput(tiles, organism.speciesNamespace));
				}

				if (SessionManager.Package.roles != null)
					report.score += FitnessManager.TestFitness(this, lastMinedTile, lastPlacedTile);
				timer = 0;
			}

			if (timeLeft > lifeTicks && npc.active == true)
			{
				if (organism != null && !SessionManager.Reports.Contains(report))
				{
					SessionManager.Reports.Add(report);
				}

				SpawnManager.spawned--;
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
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
        }

        private void PlaceBlockRight()
        {
            var pos = npc.Right.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
        }

        private void PlaceBlockBottomRight()
        {
            var pos = npc.BottomRight.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
        }

        private void PlaceBlockBottomLeft()
        {
            var pos = npc.BottomLeft.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
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
        }

        private void PlaceBlockTopLeft()
        {
            var pos = npc.TopLeft.ToTileCoordinates();
            WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt);
            lastPlacedTile = Framing.GetTileSafely(pos.X, pos.Y);
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
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockRight()
        {
            var pos = npc.Right.ToTileCoordinates();
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockBottomRight()
        {
            var pos = npc.BottomRight.ToTileCoordinates();
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockBottomLeft()
        {
            var pos = npc.BottomLeft.ToTileCoordinates();
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockBottom()
        {
            var pos = npc.Bottom.ToTileCoordinates();
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockTopRight()
        {
            var pos = npc.TopRight.ToTileCoordinates();
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockTopLeft()
        {
            var pos = npc.TopLeft.ToTileCoordinates();
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            WorldGen.KillTile(pos.X, pos.Y);
        }

        private void MineBlockTop()
        {
            var pos = npc.Top.ToTileCoordinates();
            lastMinedTile = Framing.GetTileSafely(pos.X, pos.Y);
            WorldGen.KillTile(pos.X, pos.Y);
        }

        public void DoActions(int action)
        {
            switch (action)
            {
                case 0:
                    Jump();
                    break;
                case 1:
                    MoveRight();
                    break;
                case 2:
                    MoveLeft();
                    break;
                case 3:
                    PlaceBlockTop();
                    break;
                case 4:
                    PlaceBlockTopLeft();
                    break;
                case 5:
                    PlaceBlockTopRight();
                    break;
                case 6:
                    PlaceBlockBottom();
                    break;
                case 7:
                    PlaceBlockBottomLeft();
                    break;
                case 8:
                    PlaceBlockBottomRight();
                    break;
                case 9:
                    PlaceBlockRight();
                    break;
                case 10:
                    PlaceBlockLeft();
                    break;
                case 11:
                    MineBlockTop();
                    break;
                case 12:
                    MineBlockTopLeft();
                    break;
                case 13:
                    MineBlockTopRight();
                    break;
                case 14:
                    MineBlockBottom();
                    break;
                case 15:
                    MineBlockBottomLeft();
                    break;
                case 16:
                    MineBlockBottomRight();
                    break;
                case 17:
                    MineBlockRight();
                    break;
                case 18:
                    MineBlockLeft();
                    break;
                default:
                    break;
            }
        }

        public override void DrawEffects(ref Color drawColor)
		{
			if(organism != null)
				drawColor = organism.trainingRoomRoleNamespace == "left" ? drawColor.MultiplyRGB(Color.Blue):drawColor.MultiplyRGB(Color.Red);
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


