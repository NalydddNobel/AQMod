using Aequus.UI.EventProgressBars;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Content.Events.DemonSiege {
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

        public override string GetProgressText(float progress) {

            var p = Main.LocalPlayer.Aequus().eventDemonSiege;
            var timeLeft = TextHelper.GetTextValue("TimeLeft");
            if (p == Point.Zero || !DemonSiegeSystem.ActiveSacrifices.TryGetValue(p, out var info)) {
                return timeLeft + ": X";
            }

            int seconds = info.TimeLeft / 60;
            int minutes = seconds / 60;

            if (minutes > 0) {
                return timeLeft + ": " + minutes + "m " + (seconds % 60) + "s";
            }
            return timeLeft + ": " + seconds + "s";
        }
    }
}