using ChaosTerraria.Managers;
using ChaosTerraria.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace ChaosTerraria.UI
{
    public static class UIHandler
    {
        private static bool isLoginUiVisible = false;
        private static bool isSessionUIVisible = false;
        private static bool isSpawnBlockScreenVisible = false;
        private static bool isInObserverMode = false;
        private static bool nNetDisplay = false;
        internal static int currentOrgIndex;

        public static bool IsLoginUiVisible { get => isLoginUiVisible; set => isLoginUiVisible = value; }
        public static bool IsSessionUIVisible { get => isSessionUIVisible; set => isSessionUIVisible = value; }
        public static bool IsSpawnBlockScreenVisible { get => isSpawnBlockScreenVisible; set => isSpawnBlockScreenVisible = value; }
        public static bool IsInObserverMode { get => isInObserverMode; set => isInObserverMode = value; }
        public static bool NNetDisplay { get => nNetDisplay; set => nNetDisplay = value; }

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
            IsSpawnBlockScreenVisible = true;
            ChaosSystem.mainInterface.SetState(ChaosSystem.spawnBlockScreen);
            ChaosSystem.spawnBlockScreen.GetValues(i, j);
        }

        public static void HideUI()
        {
            ChaosSystem.mainInterface.SetState(null);
        }

        public static void ToggleObserveMode()
        {
            IsInObserverMode = !IsInObserverMode;
            if (IsInObserverMode)
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
            if(!IsSpawnBlockScreenVisible && !IsLoginUiVisible && !IsSessionUIVisible)
                ChaosSystem.mainInterface.SetState(ChaosSystem.progressBar);
        }

        internal static void ShowNNetDisplay()
        {
            ChaosSystem.mainInterface.SetState(ChaosSystem.nNetDisplay);
        }
    }
}
