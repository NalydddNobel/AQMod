using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Events.GaleStreams {
    public class GaleStreamsSystem : ModSystem {
        public static byte updateTimer;
        public static bool SupressWindUpdates { get; set; }

        public override void PreUpdateEntities() {
            if (GaleStreamsZone.EventActive) {
                InnerUpdateActive();
            }
        }

        public void InnerUpdateActive() {
            if (updateTimer == 1) {
                SupressWindUpdates = false;
                for (int i = 0; i < Main.maxPlayers; i++) {
                    if (Main.player[i].active && Main.player[i].InModBiome<GaleStreamsZone>()) {
                        SupressWindUpdates = true;
                        break;
                    }
                }
            }
            updateTimer++;

            if (SupressWindUpdates) {
                Main.windCounter = Math.Max(Main.windCounter, 360);
            }
        }
    }
}