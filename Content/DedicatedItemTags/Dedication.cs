using AQMod.Items.Dedicated;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AQMod.Content.DedicatedItemTags
{
    public class Dedication : IDedicationType
    {
        public static Color YoutuberColor => new Color(250, 80, 80, 255);

        public string Text => AQText.ModText("CommonTooltip.DedicatedItem").Value;

        public void Draw(DrawableTooltipLine line)
        {
            DedicatedItemTooltips.DrawDedicatedItemText(line);
        }
    }
}