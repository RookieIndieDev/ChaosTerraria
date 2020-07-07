using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Structs;
using ChaosTerraria.UI;
using Microsoft.Xna.Framework;
using System;
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
		public static ModHotKey loginHotkey;

		private GameTime _lastUpdateUiGameTime;

		public override void Load()
		{
			loginInterface = new UserInterface();
			loginScreen = new LoginScreen();
			loginHotkey = RegisterHotKey("Login", "P");

			if (!ChaosNetConfig.CheckForConfig())
			{
				loginScreen.Activate();
				loginInterface.SetState(loginScreen);
				UIHandler.isLoginUiVisible = true;
			}
			else
			{
				try
				{
					ChaosNetConfigData configData = ChaosNetConfig.ReadConfig();
				}
				catch (Exception ex)
				{
					Logger.Error(ex.Message + ex.StackTrace);
				}
			}
		}

		public override void Unload()
		{
			loginHotkey = null;
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
						if(_lastUpdateUiGameTime != null && loginInterface.CurrentState != null)
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

			if (!UIHandler.isLoginUiVisible)
			{
				loginInterface.SetState(null);
			}

			if(loginInterface.CurrentState != null)
			{
				loginInterface.Update(gameTime);
			}
		}
	}
}