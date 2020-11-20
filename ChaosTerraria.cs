using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Managers;
using ChaosTerraria.Tiles;
using ChaosTerraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChaosTerraria
{
	public class ChaosTerraria : Mod
	{
		internal static UserInterface loginInterface;
		internal static LoginScreen loginScreen;
		internal static SessionScreen sessionScreen;
		//TODO: Add Current Stats Hotkey
		public static ModHotKey loginHotkey;
		public static ModHotKey sessionHotkey;

		private GameTime _lastUpdateUiGameTime;

		public override void Load()
		{
			loginInterface = new UserInterface();
			loginScreen = new LoginScreen();
			sessionScreen = new SessionScreen();
			loginHotkey = RegisterHotKey("Login", "P");
			sessionHotkey = RegisterHotKey("Session", "O");
			loginScreen.Activate();
			sessionScreen.Activate();

			SessionManager.InitializeSession();

			if (!ChaosNetConfig.CheckForConfig())
			{
				loginInterface.SetState(loginScreen);
				UIHandler.isLoginUiVisible = true;
			}
			else
			{
				try
				{
					ChaosNetConfig.ReadConfig();
					SessionManager.SetCurrentSessionNamespace();
				}
				catch
				{

				}
			}
		}



		public override void Unload()
		{
			loginHotkey = null;
			sessionHotkey = null;
		}

		public override void PreSaveAndQuit()
		{

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
						if (_lastUpdateUiGameTime != null && loginInterface.CurrentState != null)
						{
							loginInterface.Draw(Main.spriteBatch, new GameTime());
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

			if (!UIHandler.isLoginUiVisible && !UIHandler.isSessionUIVisible)
			{
				UIHandler.HideUI();
			}

			if (loginInterface.CurrentState != null)
			{
				loginInterface.Update(gameTime);
			}
		}
	}
}