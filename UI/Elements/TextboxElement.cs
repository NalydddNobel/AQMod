using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.UI.Elements {
    public class TextboxElement : UIState
    {
        public int maxText = -1;
        public Color color;
        public int textOffsetY = 5;

        public string text;
        public string ShowText;
        public bool focused;

        public TextboxElement(Color color = default(Color), int maxText = -1, int textOffsetY = 5)
        {
            this.color = color;
            this.maxText = maxText;
            this.textOffsetY = textOffsetY;
            text = "";
            focused = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (color == default(Color))
            {
                color = Color.White;
            }

            var dimensions = GetDimensions();

            bool hovering = Main.mouseX > dimensions.X && Main.mouseX < dimensions.Y + dimensions.Width && Main.mouseY > dimensions.Y && Main.mouseY < dimensions.Y + dimensions.Height;
            if (hovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            if (Main.mouseLeft)
            {
                focused = hovering;
            }
            if (Main.mouseRight || Terraria.GameInput.PlayerInput.Triggers.Current.Inventory)
            {
                focused = false;
            }

            if (focused)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                text = Main.GetInputText(text);
                if (maxText > 0 && text.Length > maxText)
                {
                    string newText = "";
                    for (int i = 0; i < maxText; i++)
                    {
                        newText += text[i];
                    }
                    text = newText;
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
                Main.instance.DrawWindowsIMEPanel(new Vector2(98f, Main.screenHeight - 36), 0f);
            }

            string drawnText = ShowText != null ? ShowText : text;
            ShowText = null;
            if (focused && Main.GameUpdateCount % 50 < 25)
            {
                drawnText += "|";
            }

            Utils.DrawInvBG(Main.spriteBatch, dimensions.ToRectangle(), color);

            if (drawnText != "")
            {
                var measurement = FontAssets.MouseText.Value.MeasureString(drawnText);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, drawnText,
                    new Vector2(dimensions.X + 8f, dimensions.Y + dimensions.Height / 2f - measurement.Y / 2f + textOffsetY), Color.White, 0f, Vector2.Zero, Vector2.One);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Main.UIScaleMatrix);
        }
    }
}