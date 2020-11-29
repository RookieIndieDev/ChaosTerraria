using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChaosTerraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ChaosTerraria.UI
{
    //TODO: Add Current Stats Screen
    public static class UIHandler
    {
        public static bool isLoginUiVisible = false;
        public static bool isSessionUIVisible = false;
        public static bool isSpawnBlockScreenVisible = false;

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
    }
}
