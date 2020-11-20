using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChaosTerraria;
using Terraria.ModLoader;

namespace ChaosTerraria.UI
{
    //TODO: Add Current Stats Screen
    public static class UIHandler
    {
        public static bool isLoginUiVisible = false;
        public static bool isSessionUIVisible = false;

        public static void ShowLoginScreen()
        {
            ChaosTerraria.loginInterface.SetState(ChaosTerraria.loginScreen);
        }

        public static void ShowSessionScreen()
        {
            ChaosTerraria.loginInterface.SetState(ChaosTerraria.sessionScreen);
        }

        public static void HideUI()
        {
            ChaosTerraria.loginInterface.SetState(null);
        }
    }
}
