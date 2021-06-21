using ChaosTerraria.Managers;
using ChaosTerraria.Network;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ChaosTerraria.World
{
    class ChaosWorld : ModWorld
    {
        public static HashSet<Point> spawnBlocks;
        ChaosNetworkHelper networkHelper = new ChaosNetworkHelper();

        public override void Initialize()
        {
            spawnBlocks = new HashSet<Point>();
        }


        //TODO: Don't make call to StartSession if namespace already exists?
        public override void PostUpdate()
        {
            if (!SessionManager.SessionStarted)
            {
                if (SessionManager.CurrentSession.nameSpace != null)
                {
                    networkHelper.StartSession();
                    SessionManager.SessionStarted = true;
                }
            }
            SpawnManager.SpawnTerrarians();
        }


        public static int GetSpawnBlockCount()
        {
            return spawnBlocks.Count();
        }

        public override TagCompound Save()
        {
            Point[] pointArray = new Point[spawnBlocks.Count];
            spawnBlocks.CopyTo(pointArray);
            List<Vector2> vectorList = new List<Vector2>();
            for (int i = 0; i < spawnBlocks.Count; i++)
            {
                vectorList.Add(new Vector2(pointArray[i].X, pointArray[i].Y));
            }

            return new TagCompound
            {
                { "spawnBlocks", vectorList }
            };
        }

        public override void Load(TagCompound tag)
        {
            var list = tag.GetList<Vector2>("spawnBlocks");
            Point[] pointArray = new Point[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                pointArray[i] = new Point((int)list[i].X, (int)list[i].Y);
            }
            spawnBlocks = new HashSet<Point>(pointArray);
        }
    }
}
