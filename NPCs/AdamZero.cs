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
                    DoActions(organism.nNet.GetOutput(tiles));
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
    }
}

