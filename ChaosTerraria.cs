using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Managers;
using ChaosTerraria.UI;
using ChaosTerraria.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChaosTerraria
{
    public class ChaosTerraria : Mod
    {
        public static ModKeybind loginHotkey;
        public static ModKeybind sessionHotkey;
        public static ModKeybind observerModeHotkey;
        public static ModKeybind cycleOrgs;

        public override void Load()
        {
            loginHotkey = KeybindLoader.RegisterKeybind(this, "Login", "P");
            sessionHotkey = KeybindLoader.RegisterKeybind(this, "Session", "O");
            observerModeHotkey = KeybindLoader.RegisterKeybind(this, "Observe Mode", "N");
            cycleOrgs = KeybindLoader.RegisterKeybind(this, "Cycle Orgs", "]");
            if (Main.netMode != NetmodeID.Server)
            {
                ChaosSystem.loginScreen.Activate();
                ChaosSystem.sessionScreen.Activate();
                ChaosSystem.spawnBlockScreen.Activate();
                ChaosSystem.progressBar.Activate();
                SessionManager.InitializeSession();

                if (!ChaosNetConfig.CheckForConfig())
                {
                    ChaosSystem.mainInterface.SetState(ChaosSystem.loginScreen);
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
            cycleOrgs = null;
        }


    }
}