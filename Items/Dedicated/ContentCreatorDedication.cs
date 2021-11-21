using AQMod.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Dedicated
{
    public class ContentCreatorDedication : IDedicationType
    {
        public string Text => AQText.ModText("CommonTooltip.ContentCreatorItem").Value;

        public void Draw(DrawableTooltipLine line)
        {
            DedicatedItemTooltips.DrawDedicatedItemText(line);
        }
    }
}