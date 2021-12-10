using AQMod.Assets;
using AQMod.Common.Graphics.DrawTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.Localization;

namespace AQMod.Common.Graphics.CustomPickupText
{
    public class NarrizuulPickupDrawObject : IDrawObject // unfinished
    {
        public string key;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public float rotation;
        public float scale;
        public int lifeTime;

        public NarrizuulPickupDrawObject(string text, Vector2 position, Vector2 velocity, Color color, float rotation = 0f, float scale = 1f, int lifeTime = 240)
        {
            key = text;
            this.position = position;
            this.velocity = velocity;
            this.color = color;
            this.rotation = rotation;
            this.scale = scale;
            this.lifeTime = lifeTime;
        }

        void IDrawType.RunDraw()
        {
            float targetScale = ItemText.TargetScale;
            if (targetScale == 0f)
                targetScale = 1f;
            string text = Language.GetTextValue(key);
            Vector2 textSize = Main.fontMouseText.MeasureString(text);
            Vector2 origin = textSize / 2f;
            float scale = this.scale / targetScale;
            float y = position.Y - Main.screenPosition.Y;
            if (Main.player[Main.myPlayer].gravDir == -1f)
                y = Main.screenHeight - y;

            float off = 2f * scale;
            var borderClr = Color.Black;
            var drawPos = new Vector2(position.X - Main.screenPosition.X, y + origin.Y);
            Main.spriteBatch.DrawString(Main.fontMouseText, text, new Vector2(position.X - Main.screenPosition.X - off, y + origin.Y), borderClr, rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, text, new Vector2(position.X - Main.screenPosition.X + off, y + origin.Y), borderClr, rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, text, new Vector2(position.X - Main.screenPosition.X, y + origin.Y + off), borderClr, rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, text, new Vector2(position.X - Main.screenPosition.X, y + origin.Y - off), borderClr, rotation, origin, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.DrawString(Main.fontMouseText, text, drawPos, color, rotation, origin, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.DrawString(Main.fontMouseText, text, drawPos + new Vector2(off, 0).RotatedBy(Main.GlobalTime), new Color(255, 0, 0, 0), rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, text, drawPos + new Vector2(off, 0).RotatedBy(Main.GlobalTime + MathHelper.TwoPi / 6), new Color(255, 255, 0, 0), rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, text, drawPos + new Vector2(off, 0).RotatedBy(Main.GlobalTime + MathHelper.TwoPi / 6 * 2f), new Color(0, 0, 255, 0), rotation, origin, scale, SpriteEffects.None, 0f);

            Texture2D texture = AQTextures.Lights[LightTex.Spotlight12x66];
            Main.spriteBatch.Draw(texture, drawPos + new Vector2((float)Math.Sin(Main.GlobalTime * 2.1111f), -4f + (float)Math.Sin(Main.GlobalTime * 2.3134f)), null, new Color(0, 70, 0, 0), rotation + MathHelper.PiOver2, new Vector2(6f, 33f), new Vector2(1.2f + (float)Math.Sin(Main.GlobalTime * 4f) * 0.145f, origin.Y * 0.15f) * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos + new Vector2((float)Math.Sin(Main.GlobalTime * 2.1111f + 5245f) * 4f, -4f + (float)Math.Sin(Main.GlobalTime * 2.3134f + 12f) * 2f), null, new Color(70, 70, 0, 0), rotation + MathHelper.PiOver2, new Vector2(6f, 33f), new Vector2(1.2f + (float)Math.Sin(Main.GlobalTime + 655f) * 0.2f, origin.Y * 0.1435f) * scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos + new Vector2((float)Math.Sin(Main.GlobalTime * 2.1111f + 12f) * -4f, -4f + (float)Math.Sin(Main.GlobalTime * 2.3134f + 5245f) * -1.25f), null, new Color(0, 0, 70, 0), rotation + MathHelper.PiOver2, new Vector2(6f, 33f), new Vector2(1.2f + (float)Math.Sin(Main.GlobalTime * 2f + 777f) * 0.2f, origin.Y * 0.25f) * scale, SpriteEffects.None, 0f);
        }

        bool IDrawObject.Update()
        {
            rotation = (float)Math.Sin(Main.GlobalTime * 2f) * 0.15f;
            if (lifeTime < 0)
                scale -= 0.01f;
            else
            {
                if (scale < 1f)
                    scale += 0.1f;
                lifeTime--;
            }
            position += velocity;
            velocity *= 0.85f;
            return scale < 0.1f;
        }
    }
}