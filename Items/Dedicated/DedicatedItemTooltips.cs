using AQMod.Localization;
using Microsoft.Xna.Framework;
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
                tooltips.Add(new TooltipLine(ModContent.GetInstance<AQMod>(), "DedicatedItem", AQText.ModText("Common.DedicatedItemTag").Value) { overrideColor = dedicated.DedicatedItemColor() });
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "AQMod" && line.Name == "DedicatedItem")
            {
                Color color = Colors.AlphaDarken(line.overrideColor.GetValueOrDefault(line.color));
                color.A = 0;
                float xOff = (float)Math.Sin(Main.GlobalTime * 15f) + 1f;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, line.text, new Vector2(line.X, line.Y), new Color(0, 0, 0, 255), line.rotation, line.origin, line.baseScale);
                ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, line.text, new Vector2(line.X, line.Y), color, line.rotation, line.origin, line.baseScale);
                ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, line.text, new Vector2(line.X, line.Y), color, line.rotation, line.origin, line.baseScale);
                ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, line.text, new Vector2(line.X, line.Y), color * 0.8f, line.rotation, line.origin, line.baseScale);
                ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, line.text, new Vector2(line.X, line.Y), color * 0.8f, line.rotation, line.origin, line.baseScale);
                return false;
            }
            return true;
        }
    }
}