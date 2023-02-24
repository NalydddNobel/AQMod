using Aequus.UI.EventProgressBars;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Events.DemonSiege
{
    public class DemonSiegeProgressBar : LegacyEventProgressBar
    {
        public override bool IsActive()
        {
            return Main.LocalPlayer.Aequus().ZoneDemonSiege && DemonSiegeSystem.DemonSiegePause <= 0;
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