using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Items.Dyes.Hair
{
    public class TemperatureHairDye : HairDyeItem
    {
        protected override Color GetHairClr(Player player, Color hairColor, ref bool useLighting)
        {
            sbyte temperature = player.GetModPlayer<AQPlayer>().temperature;
            return Color.Lerp(new Color(20, 60, 255, 255), new Color(255, 60, 20, 255), (temperature / 100f + 1f) / 2f);
        }
    }
}