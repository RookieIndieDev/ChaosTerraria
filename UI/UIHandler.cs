using ChaosTerraria.Managers;
using ChaosTerraria.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace ChaosTerraria.UI
{
    //TODO: Add Current Stats Screen
    public static class UIHandler
    {
        public static bool isLoginUiVisible = false;
        public static bool isSessionUIVisible = false;
        public static bool isSpawnBlockScreenVisible = false;
        public static bool isInObserverMode = false;
        internal static int currentOrgIndex;

        public static void ShowLoginScreen()
        {
            ChaosSystem.mainInterface.SetState(ChaosSystem.loginScreen);
        }

        public static void ShowSessionScreen()
        {
            ChaosSystem.mainInterface.SetState(ChaosSystem.sessionScreen);
        }

        public static void ShowSpawnBlockScreen(int i, int j)
        {
            isSpawnBlockScreenVisible = true;
            ChaosSystem.mainInterface.SetState(ChaosSystem.spawnBlockScreen);
            ChaosSystem.spawnBlockScreen.GetValues(i, j);
        }

        public static void HideUI()
        {
            ChaosSystem.mainInterface.SetState(null);
        }

        public static void ToggleObserveMode()
        {
            isInObserverMode = !isInObserverMode;
            if (isInObserverMode)
            {
                Main.NewText("Observer Mode activated", Color.LightBlue);
                if (SessionManager.ObservableNPCs != null && SessionManager.ObservableNPCs.Count > 0)
                    currentOrgIndex = 0;
            }
            else
            {
                Main.NewText("Observer Mode deactivated", Color.LightBlue);
            }
        }

        internal static void ShowProgressBar()
        {
            if(!isSpawnBlockScreenVisible && !isLoginUiVisible && !isSessionUIVisible)
                ChaosSystem.mainInterface.SetState(ChaosSystem.progressBar);
        }
    }
}
