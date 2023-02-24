using Aequus.UI.EventProgressBars;
using System;
using Terraria;

namespace Aequus.Events.Glimmer
{
    public class GlimmerProgressBar : LegacyEventProgressBar
    {
        public override bool IsActive()
        {
            return Main.LocalPlayer.Aequus().ZoneGlimmer && GlimmerBiome.omegaStarite == -1;
        }

        public override float GetEventProgress()
        {
            return Math.Clamp(1f - GlimmerSystem.CalcTiles(Main.LocalPlayer) / (float)GlimmerBiome.MaxTiles, 0f, 1f);
        }
    }
}