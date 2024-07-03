using System;

namespace Aequu2.Content.Events.GaleStreams;

public class GaleStreamsSystem : ModSystem {
    private byte _updateTimer;
    public static bool SupressWindUpdates { get; set; }

    public override void PreUpdateEntities() {
        if (!GaleStreamsZone.EventActive) {
            return;
        }

        if (_updateTimer == 1) {
            SupressWindUpdates = false;
            for (int i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && Main.player[i].InModBiome<GaleStreamsZone>()) {
                    SupressWindUpdates = true;
                    break;
                }
            }
        }
        _updateTimer++;

        if (SupressWindUpdates) {
            Main.windCounter = Math.Max(Main.windCounter, 360);
        }
    }
}