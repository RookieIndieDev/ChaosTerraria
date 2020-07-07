using ChaosTerraria.Managers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosTerraria.NPCs
{
	public class ChaosTerrarian : ModNPC
	{
		public override string Texture => "ChaosTerraria/NPCs/Terrarian";
		private static int timer = 0;
		private int timeLeft = 0;
		private int[] tiles = new int[25];

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("ChaosTerrarian");
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
			timer++;
			timeLeft++;

			if (timer > 15 && npc.active == true)
			{
				DoScan(npc.position);
				DoActions();
				timer = 0;
			}

			if (timeLeft > 600 && npc.active == true)
			{
				SpawnManager.spawned--;
				timeLeft = 0;
				Dust.NewDust(npc.position, 2, 2, 1);
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
				npc.velocity.Y = -7f;
			}
		}

		public void DoActions()
		{
			int action = Main.rand.Next(1, 4);
			switch (action)
			{
				case 1:
					MoveRight();
					break;
				case 2:
					MoveLeft();
					break;
				case 3:
					Jump();
					break;
			}
		}

		private void DoScan(Vector2 position)
		{
			int range = 2;
			int blockCount = 0;
			Point startPoint = position.ToTileCoordinates();
			for (int i = startPoint.X - range; i < startPoint.X + range; i++)
			{
				for (int j = startPoint.Y - range; j < startPoint.Y + range; j++)
				{
					tiles[blockCount++] = Main.tile[i, j].type;
				}
			}
		}
	}
}


