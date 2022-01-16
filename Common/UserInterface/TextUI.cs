using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI.Chat;

namespace AQMod.Common.UserInterface
{
    public class TextUI
    {
        public string text;
        public bool focused;

        public TextUI()
        {
            text = "";
        }

        public void Draw(int x, int y, int width, int height, int maxText = -1, Color color = default(Color), int textOffsetY = 5)
        {
            if (color == default(Color))
            {
                color = Color.White;
            }
            bool hovering = Main.mouseX > x && Main.mouseX < x + width && Main.mouseY > y && Main.mouseY < y + height;
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

            string drawnText = text;
            if (focused && Main.GameUpdateCount % 50 < 25)
            {
                drawnText += "|";
            }

            Utils.DrawInvBG(Main.spriteBatch, new Rectangle(x, y, width, height), color);

            if (drawnText != "")
            {
                var measurement = Main.fontMouseText.MeasureString(drawnText);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, drawnText, new Vector2(x + 8f, y + height / 2f - measurement.Y / 2f + textOffsetY), Color.White, 0f, Vector2.Zero, Vector2.One);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Main.UIScaleMatrix);
        }
    }
}