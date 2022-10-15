using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChaosTerraria.UI
{
    internal class NNetDisplay : UIState
    {
        UIImage neuron;
        UIPanel mainPanel;
        Texture2D line;
        Rectangle target;

        public override void OnInitialize()
        {
            mainPanel = new();
            mainPanel = new UIPanel
            {
                BackgroundColor = Color.Transparent,
                BorderColor = Color.White
            };
            neuron = new(ModContent.Request<Texture2D>("ChaosTerraria/UI/Neuron", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            line = ModContent.Request<Texture2D>("ChaosTerraria/UI/ProgressBar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            mainPanel.Width.Set(Main.screenWidth * 0.50f, 0f);
            mainPanel.Height.Set(Main.screenHeight * 0.65f, 0f);
            mainPanel.Top.Set(Main.screenHeight * 0.25f, 0f);
            mainPanel.Left.Set(Main.screenWidth * 0.25f, 0f);
            neuron.Top.Set(10, 0f);
            neuron.Left.Set(10, 0f);
            mainPanel.Append(neuron);
            Append(mainPanel);
            target = mainPanel.GetInnerDimensions().ToRectangle();
            target.Height = 10;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(line, target.Center.ToVector2(), null, Color.White, 1.57f, target.Center.ToVector2(), Vector2.One, SpriteEffects.None, 0f);
            spriteBatch.Draw(line, new Rectangle(target.X + 1, target.Y-2, 100, 5),null, Color.White, (float)Math.PI/4, target.Center.ToVector2(), SpriteEffects.None, 1f);
            base.DrawSelf(spriteBatch);
        }
    }
}
