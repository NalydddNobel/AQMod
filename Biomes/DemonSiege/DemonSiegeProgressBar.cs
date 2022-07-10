using Aequus.UI.EventProgressBars;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Biomes.DemonSiege
{
    public class DemonSiegeProgressBar : EventProgressBar
    {
        public override bool IsActive()
        {
            return Main.LocalPlayer.InModBiome<DemonSiegeBiome>();
        }

        public override float GetEventProgress()
        {
            var p = Main.LocalPlayer.Aequus().eventDemonSiege;
            if (p == Point.Zero || !DemonSiegeSystem.ActiveSacrifices.TryGetValue(p, out var info))
            {
                return 0f;
            }
            return 1f - info.TimeLeft / (float)info.TimeLeftMax;
        }
    }
}