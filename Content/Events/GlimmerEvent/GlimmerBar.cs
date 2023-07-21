using Aequus.Common.UI.EventBars;
using System;
using Terraria;

namespace Aequus.Content.Events.GlimmerEvent {
    public class GlimmerBar : EventBar {
        public override bool IsActive() {
            return Main.LocalPlayer.InModBiome<GlimmerZone>() && GlimmerZone.omegaStarite == -1;
        }

        public override float GetEventProgress() {
            return Math.Clamp(1f - GlimmerSystem.GetTileDistance(Main.LocalPlayer) / (float)GlimmerZone.MaxTiles, 0f, 1f);
        }

        public override string GetProgressText(float progress) {

            string distance = TextHelper.GetTextValue("Distance");
            string blocksAway = TextHelper.GetTextValue("BlocksAway");

            return distance + ": " + GlimmerSystem.GetTileDistance(Main.LocalPlayer) + " " + blocksAway;
        }
    }
}