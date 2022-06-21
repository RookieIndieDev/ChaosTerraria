using ChaosTerraria.Managers;
using ChaosTerraria.UI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace ChaosTerraria.World
{
    class ChaosSystem : ModSystem
    {
        public static HashSet<Point> spawnBlocks;
        internal static UserInterface mainInterface;
        internal static LoginScreen loginScreen;
        internal static SessionScreen sessionScreen;
        internal static SpawnBlockScreen spawnBlockScreen;
        internal static GenProgressBar progressBar;
        private GameTime _lastUpdateUiGameTime;

        public override void Load()
        {
            mainInterface = new UserInterface();
            loginScreen = new LoginScreen();
            sessionScreen = new SessionScreen();
            spawnBlockScreen = new SpawnBlockScreen();
            progressBar = new GenProgressBar();
            spawnBlocks = new HashSet<Point>();
            base.Load();
        }

        public override void PostUpdateWorld()
        {
            SpawnManager.SpawnTerrarians();
        }

        public static int GetSpawnBlockCount()
        {
            return spawnBlocks.Count;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            Point[] pointArray = new Point[spawnBlocks.Count];
            spawnBlocks.CopyTo(pointArray);
            List<Vector2> vectorList = new();
            for (int i = 0; i < spawnBlocks.Count; i++)
            {
                vectorList.Add(new Vector2(pointArray[i].X, pointArray[i].Y));
            }
            tag.Set("spawnBlocks", vectorList);
            File.WriteAllText("weight.json", JsonConvert.SerializeObject(ChaosTerraria.weight));
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Items.SpawnBlock>(), 999);
            recipe.AddIngredient(ItemID.Wood);
            recipe.Register();
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var list = tag.GetList<Vector2>("spawnBlocks");
            Point[] pointArray = new Point[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                pointArray[i] = new Point((int)list[i].X, (int)list[i].Y);
            }
            spawnBlocks = new HashSet<Point>(pointArray);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;

            if (!UIHandler.IsLoginUiVisible && !UIHandler.IsSessionUIVisible && !UIHandler.IsSpawnBlockScreenVisible)
            {
                UIHandler.HideUI();
            }

            if (mainInterface.CurrentState != null)
            {
                mainInterface.Update(gameTime);
            }

            if (SessionManager.SessionStarted)
            {
                UIHandler.ShowProgressBar();
                progressBar.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "ChaosNet Login Interface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && mainInterface.CurrentState != null)
                        {
                            mainInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
