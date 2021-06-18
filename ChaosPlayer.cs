using Terraria.GameInput;
using Terraria.ModLoader;
using ChaosTerraria.UI;

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
