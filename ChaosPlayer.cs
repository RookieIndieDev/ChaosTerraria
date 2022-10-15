using Terraria.GameInput;
using Terraria.ModLoader;
using ChaosTerraria.UI;
using Terraria;
using ChaosTerraria.Managers;

namespace ChaosTerraria
{
    class ChaosPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ChaosTerraria.loginHotkey.JustPressed)
			{
                UIHandler.IsLoginUiVisible = !UIHandler.IsLoginUiVisible;

                if (UIHandler.IsLoginUiVisible)
                {
                    UIHandler.ShowLoginScreen();
                }
			}

            if (ChaosTerraria.sessionHotkey.JustPressed)
            {
                UIHandler.IsSessionUIVisible = !UIHandler.IsSessionUIVisible;

                if (UIHandler.IsSessionUIVisible)
                {
                    UIHandler.ShowSessionScreen();
                }
            }

            if (ChaosTerraria.observerModeHotkey.JustPressed)
            {
                UIHandler.ToggleObserveMode();
            }

            if(ChaosTerraria.cycleOrgs.JustPressed)
            {
                if(SessionManager.ObservableNPCs != null && SessionManager.ObservableNPCs.Count > 0)
                {
                    if(UIHandler.currentOrgIndex + 1 == SessionManager.ObservableNPCs.Count)
                    {
                        UIHandler.currentOrgIndex = 0;
                    }
                    else
                    {
                        UIHandler.currentOrgIndex++;
                    }
                }
            }
            if (ChaosTerraria.nNetDisplay.JustPressed)
            {
                UIHandler.NNetDisplay = !UIHandler.NNetDisplay;
                if (UIHandler.NNetDisplay)
                {
                    UIHandler.ShowNNetDisplay();
                }
            }
        }

        public override void ModifyScreenPosition()
        {
            if(SessionManager.ObservableNPCs != null && SessionManager.ObservableNPCs.Count > 0 && UIHandler.IsInObserverMode)
            {
                Main.screenPosition.X = SessionManager.ObservableNPCs[UIHandler.currentOrgIndex].NPC.Center.X - Main.screenWidth / 2;
                Main.screenPosition.Y = SessionManager.ObservableNPCs[UIHandler.currentOrgIndex].NPC.Center.Y - Main.screenHeight / 2;
            }
        }
    }
}
