﻿using ChaosTerraria.Classes;
using ChaosTerraria.Fitness;
using ChaosTerraria.Managers;
using ChaosTerraria.Structs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
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
					DoActions(organism.nNet.GetOutput(tiles));
				}

				if (SessionManager.Package.roles != null)
					report.score += FitnessManager.TestFitness(this);
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
                default:
                    Main.NewText("Invalid Action");
                    break;
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
	}
}


