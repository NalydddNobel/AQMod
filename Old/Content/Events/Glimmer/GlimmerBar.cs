using Aequus.Old.Common.EventBars;
using System;
using Terraria.Localization;

namespace Aequus.Old.Content.Events.Glimmer;

public class GlimmerBar : EventBar {
    public override bool IsActive() {
        return Main.LocalPlayer.InModBiome<GlimmerZone>() && GlimmerZone.omegaStarite == -1;
    }

    public override float GetEventProgress() {
        return Math.Clamp(1f - GlimmerSystem.GetTileDistance(Main.LocalPlayer) / (float)GlimmerZone.MaxTiles, 0f, 1f);
    }

    public override string GetProgressText(float progress) {
        return Language.GetText("Mods.Aequus.Misc.BlocksDistance").Format(GlimmerSystem.GetTileDistance(Main.LocalPlayer));
    }
}