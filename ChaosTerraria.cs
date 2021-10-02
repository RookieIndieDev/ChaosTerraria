using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Managers;
using ChaosTerraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChaosTerraria
{
    public class ChaosTerraria : Mod
    {
        internal static UserInterface mainInterface;
        internal static LoginScreen loginScreen;
        internal static SessionScreen sessionScreen;
        internal static SpawnBlockScreen spawnBlockScreen;
        //TODO: Add Current Stats Hotkey
        public static ModHotKey loginHotkey;
        public static ModHotKey sessionHotkey;
        public static ModHotKey observerModeHotkey;

        private GameTime _lastUpdateUiGameTime;

        public override void Load()
        {
            mainInterface = new UserInterface();
            loginScreen = new LoginScreen();
            sessionScreen = new SessionScreen();
            spawnBlockScreen = new SpawnBlockScreen();
            loginHotkey = RegisterHotKey("Login", "P");
            sessionHotkey = RegisterHotKey("Session", "O");
            observerModeHotkey = RegisterHotKey("Observe Mode", "N");
            if (Main.netMode != NetmodeID.Server)
            {
                loginScreen.Activate();
                sessionScreen.Activate();
                spawnBlockScreen.Activate();

                SessionManager.InitializeSession();

                if (!ChaosNetConfig.CheckForConfig())
                {
                    mainInterface.SetState(loginScreen);
                    UIHandler.isLoginUiVisible = true;
                }
                else
                {
                    ChaosNetConfig.ReadConfig();
                    SessionManager.SetCurrentSessionNamespace();
                }
            }
        }

        public override void Unload()
        {
            loginHotkey = null;
            sessionHotkey = null;
            observerModeHotkey = null;
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

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;

            if (!UIHandler.isLoginUiVisible && !UIHandler.isSessionUIVisible && !UIHandler.isSpawnBlockScreenVisible)
            {
                UIHandler.HideUI();
            }

            if (mainInterface.CurrentState != null)
            {
                mainInterface.Update(gameTime);
            }
        }
    }
}