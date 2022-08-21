using ChaosTerraria.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChaosTerraria.UI
{
    class GenProgressBar : UIState
    {
        UIText percent;
        UIText content;
        UIPanel mainPanel;
        Texture2D progressBar;
        Rectangle target;
        public override void OnInitialize()
        {
            mainPanel = new UIPanel
            {
                BackgroundColor = Color.Transparent,
                BorderColor = Color.White
            };
            percent = new UIText("");
            content = new UIText("Generation Progress");
            progressBar = ModContent.Request<Texture2D>("ChaosTerraria/UI/ProgressBar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            mainPanel.Width.Set(Main.screenWidth * 0.20f, 0f);
            mainPanel.Height.Set(100f, 0f);
            mainPanel.Top.Set(Main.screenHeight * 0.55f, 0f);
            mainPanel.Left.Set(Main.screenWidth * 0.70f, 0f);
            content.Top.Set(mainPanel.Top.Pixels * 0.1f, 0f);
            percent.Top.Set(mainPanel.Top.Pixels * 0.01f, 0f);
            mainPanel.Append(percent);
            mainPanel.Append(content);
            Append(mainPanel);
            target = mainPanel.GetInnerDimensions().ToRectangle();
            target.X += 5;
            target.Y += 30;
            target.Height = 10;
            target.Width = 0;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            target.Width = (int)Math.Min(target.Width + 5, (mainPanel.GetInnerDimensions().ToRectangle().Width * 0.85 * (SessionManager.PercentCompleted/100f)));
            percent.SetText(SessionManager.PercentCompleted + " %");
            spriteBatch.Draw(progressBar,target, Color.White); 
            base.DrawSelf(spriteBatch);
        }
    }
}
