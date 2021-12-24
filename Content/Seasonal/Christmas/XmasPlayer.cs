using Terraria.ModLoader;

namespace AQMod.Content.Seasonal.Christmas
{
    public class XmasPlayer : ModPlayer
    {
        public override void UpdateBiomes()
        {
            if (XmasSeeds.XmasWorld)
            {
                player.ZoneSnow = true;
            }
        }
    }
}