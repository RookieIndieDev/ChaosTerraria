using Microsoft.Xna.Framework;
using Terraria;

namespace ChaosTerraria.UI
{
    //TODO: Add Current Stats Screen
    //TODO: Finish observe mode up
    public static class UIHandler
    {
        public static bool isLoginUiVisible = false;
        public static bool isSessionUIVisible = false;
        public static bool isSpawnBlockScreenVisible = false;
        public static bool isInObserverMode = false;

        public static void ShowLoginScreen()
        {
            ChaosTerraria.mainInterface.SetState(ChaosTerraria.loginScreen);
        }

        public static void ShowSessionScreen()
        {
            ChaosTerraria.mainInterface.SetState(ChaosTerraria.sessionScreen);
        }

        public static void ShowSpawnBlockScreen(int i, int j)
        {
            isSpawnBlockScreenVisible = true;
            ChaosTerraria.mainInterface.SetState(ChaosTerraria.spawnBlockScreen);
            ChaosTerraria.spawnBlockScreen.GetValues(i, j);
        }

        public static void HideUI()
        {
            ChaosTerraria.mainInterface.SetState(null);
        }

        public static void ToggleObserveMode()
        {
            isInObserverMode = !isInObserverMode;

            if (isInObserverMode)
            {
                Main.NewText("Observer Mode activated", Color.Blue);
            }
            else
            {
                Main.NewText("Observer Mode deactivated", Color.Blue);
            }
        }
    }
}
