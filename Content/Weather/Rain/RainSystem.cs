using System;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;

namespace Aequus.Content.Weather.Rain;

public class RainSystem : ModSystem {
    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
        RainTotem.RainTotemCount = tileCounts[ModContent.TileType<RainTotem>()] / 4;
    }

    public override void PostUpdateWorld() {
        base.PostUpdateWorld();
    }

    public override void PostUpdateTime() {
        // Vanilla conditions for preventing rain checks
        if (Main.raining || Main.slimeRain || LanternNight.LanternsUp || LanternNight.NextNightIsLanternNight || CreativePowerManager.Instance.GetPower<CreativePowers.FreezeRainPower>().Enabled) {
            return;
        }

        RainTotem.UpdateRainState();
    }
}
