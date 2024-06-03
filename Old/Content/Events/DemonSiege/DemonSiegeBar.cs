using Aequus.Old.Common.EventBars;
using Terraria.Localization;

namespace Aequus.Old.Content.Events.DemonSiege;

public class DemonSiegeBar : EventBar {
    public override bool IsActive() {
        return Main.LocalPlayer.InModBiome<DemonSiegeZone>() && DemonSiegeSystem.DemonSiegePause <= 0;
    }

    public override float GetEventProgress() {
        Point p = Main.LocalPlayer.GetModPlayer<DemonSiegePlayer>().GoreNest;
        if (p == Point.Zero || !DemonSiegeSystem.ActiveSacrifices.TryGetValue(p, out var info)) {
            return 0f;
        }
        return 1f - info.TimeLeft / (float)info.TimeLeftMax;
    }

    public override string GetProgressText(float progress) {
        Point p = Main.LocalPlayer.GetModPlayer<DemonSiegePlayer>().GoreNest;
        LocalizedText timeLeft = Language.GetText("Mods.Aequus.Misc.TimeLeft");
        if (p == Point.Zero || !DemonSiegeSystem.ActiveSacrifices.TryGetValue(p, out var info)) {
            return timeLeft.Format("X");
        }

        int seconds = info.TimeLeft / 60;
        int minutes = seconds / 60;

        if (minutes > 0) {
            return timeLeft.Format(minutes + "m " + seconds % 60 + "s");
        }
        return timeLeft.Format(seconds + "s");
    }
}