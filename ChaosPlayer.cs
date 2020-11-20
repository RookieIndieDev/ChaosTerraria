using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using Terraria.ModLoader;
using ChaosTerraria;
using ChaosTerraria.UI;
using Terraria;

namespace ChaosTerraria
{
    class ChaosPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ChaosTerraria.loginHotkey.JustPressed)
			{
                UIHandler.isLoginUiVisible = !UIHandler.isLoginUiVisible;

                if (UIHandler.isLoginUiVisible)
                {
                    UIHandler.ShowLoginScreen();
                }
			}

            if (ChaosTerraria.sessionHotkey.JustPressed)
            {
                UIHandler.isSessionUIVisible = !UIHandler.isSessionUIVisible;

                if (UIHandler.isSessionUIVisible)
                {
                    UIHandler.ShowSessionScreen();
                }
            }
		}
    }
}
