using AQMod.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Dedicated
{
    public class ContributorDedication : IDedicationType
    {
        public string Text => AQText.ModText("CommonTooltip.DedicatedItem").Value;

        public void Draw(DrawableTooltipLine line)
        {
            DedicatedItemTooltips.DrawDedicatedItemText(line);
        }
    }
}