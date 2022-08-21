using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace ChaosTerraria.UI
{
    class ModTextBox : UITextBox
    {
        private bool focused = false;
        public event Action OnFocus;
        public event Action OnUnfocus;

        public ModTextBox(string text, float textScale = 1, bool large = false) : base(text, textScale, large)
        {

        }

        public override void Click(UIMouseEvent evt)
        {
            Focus();
            base.Click(evt);
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 mousePosition = new(Main.mouseX, Main.mouseY);
            if (!ContainsPoint(mousePosition) && Main.mouseLeft)
            {
                Unfocus();
            }
            base.Update(gameTime);
        }

        private void Focus()
        {
            if (!focused)
            {
                focused = true;
                Main.blockInput = true;
                Main.clrInput();

                OnFocus?.Invoke();
            }
        }

        private void Unfocus()
        {
            if (focused)
            {
                focused = false;
                Main.blockInput = false;

                OnUnfocus?.Invoke();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (focused)
            {
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                base.SetText(Main.GetInputText(Text));
            }

            base.DrawSelf(spriteBatch);
        }
    }
}
