using AQMod.Assets;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AQMod.Items.Dedicated
{
    public class DedicatedItemTooltips : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.modItem is IDedicatedItem dedicated)
            {
                tooltips.Add(new TooltipLine(mod, "DedicatedItem", dedicated.DedicationType.Text) { overrideColor = dedicated.DedicatedItemColor });
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "AQMod" && line.Name == "DedicatedItem")
            {
                (item.modItem as IDedicatedItem).DedicationType.Draw(line);
                return false;
            }
            return true;
        }

        public static void DrawDedicatedItemText(DrawableTooltipLine line)
        {
            DrawDedicatedItemText(line.text, line.X, line.Y, line.rotation, line.origin, line.baseScale, line.overrideColor.GetValueOrDefault(line.color));
        }

        public static void DrawDedicatedItemText(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            color = Colors.AlphaDarken(color);
            color.A = 0;
            float xOff = (float)Math.Sin(Main.GlobalTime * 15f) + 1f;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), new Color(0, 0, 0, 255), rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color, rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color, rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color * 0.8f, rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color * 0.8f, rotation, origin, baseScale);
        }

        public static void DrawNarrizuulText(DrawableTooltipLine line)
        {
            DrawNarrizuulText(line.text, line.X, line.Y, line.rotation, line.origin, line.baseScale);
        }

        public static void DrawNarrizuulText(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale)
        {
            var font = Main.fontMouseText;
            Vector2 center = font.MeasureString(text) / 2f;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(x, y), Color.Black, rotation, origin, baseScale);
            var offset = new Vector2(2f, 0);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x, y) + offset.RotatedBy(Main.GlobalTime), new Color(255, 0, 0, 0), 
                rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x, y) + offset.RotatedBy(Main.GlobalTime + MathHelper.TwoPi / 6), new Color(255, 255, 0, 0), 
                rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x, y) + offset.RotatedBy(Main.GlobalTime + MathHelper.TwoPi / 6 * 2), new Color(0, 0, 255, 0), 
                rotation, origin, baseScale);

            if (AQConfigClient.c_EffectQuality < 1f)
                return;

            var texture = AQTextures.Lights[LightTex.Spotlight12x66];
            var spotlightOrigin = new Vector2(6f, 33f);
            float spotlightRotation = rotation + MathHelper.PiOver2;
            var spotlightEffect = SpriteEffects.None;
            Main.spriteBatch.Draw(texture, new Vector2(x, y) + center + 
                new Vector2((float)Math.Sin(Main.GlobalTime * 2.1111f), -4f + 
                (float)Math.Sin(Main.GlobalTime * 2.3134f)), null, new Color(0, 70, 0, 0), spotlightRotation,
               spotlightOrigin, new Vector2(1.2f + (float)Math.Sin(Main.GlobalTime * 4f) * 0.145f, center.Y * 0.15f), spotlightEffect, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y) + center + 
                new Vector2((float)Math.Sin(Main.GlobalTime * 2.1111f + 5245f) * 4f, -4f + 
                (float)Math.Sin(Main.GlobalTime * 2.3134f + 12f) * 2f), null, new Color(70, 70, 0, 0), spotlightRotation,
                spotlightOrigin, new Vector2(1.2f + (float)Math.Sin(Main.GlobalTime + 655f) * 0.2f, center.Y * 0.1435f), spotlightEffect, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y) + center + 
                new Vector2((float)Math.Sin(Main.GlobalTime * 2.1111f + 12f) * -4f, -4f + 
                (float)Math.Sin(Main.GlobalTime * 2.313f + 5245f) * -1.25f), null, new Color(0, 0, 70, 0), spotlightRotation,
                spotlightOrigin, new Vector2(1.2f + (float)Math.Sin(Main.GlobalTime * 2f + 777f) * 0.2f, center.Y * 0.25f), spotlightEffect, 0f);
        }
    }
}