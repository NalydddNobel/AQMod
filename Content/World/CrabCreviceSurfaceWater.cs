using AQMod.Dusts.Splashes;
using AQMod.Gores;
using AQMod.Gores.Droplets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.World
{
    public sealed class CrabCreviceSurfaceWater : ModWaterStyle
    {
        public override bool ChooseWaterStyle()
        {
            var aQPlayer = Main.player[Main.myPlayer].GetModPlayer<AQPlayer>();
            return aQPlayer.ZoneCrabSeason || (aQPlayer.ZoneCrabCrevice && Main.player[Main.myPlayer].position.Y < Main.worldSurface * 16f);
        }

        public override int ChooseWaterfallStyle()
            => ModContent.GetInstance<CrabCreviceSurfaceWaterfall>().Type;

        public override int GetSplashDust()
            => ModContent.DustType<CrabSeasonSplash>();

        public override int GetDropletGore()
            => AQGore.GetID<CrabSeasonDroplet>();

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 1f;
            b = 1f;
        }

        public override Color BiomeHairColor()
            => Color.SandyBrown;
    }
}