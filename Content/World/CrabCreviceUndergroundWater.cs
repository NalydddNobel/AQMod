using AQMod.Content.Players;
using AQMod.Dusts.Splashes;
using AQMod.Gores;
using AQMod.Gores.Droplets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.World
{
    public sealed class CrabCreviceUndergroundWater : ModWaterStyle
    {
        public override bool ChooseWaterStyle()
        {
            return Main.player[Main.myPlayer].GetModPlayer<PlayerBiomes>().zoneCrabCrevice && Main.player[Main.myPlayer].position.Y >= Main.worldSurface * 16f;
        }

        public override int ChooseWaterfallStyle()
            => ModContent.GetInstance<CrabCreviceUndergroundWaterfall>().Type;

        public override int GetSplashDust()
            => ModContent.DustType<CrabCreviceSplash>();

        public override int GetDropletGore()
            => AQGore.GetID<CrabCreviceDroplet>();

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