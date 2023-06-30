using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.NPCs {
    public class FlawlessFightSystem : ModSystem {
        public static List<byte> DamagedPlayers { get; private set; }

        public override void Load() {
            DamagedPlayers = new List<byte>();
        }

        public override void Unload() {
            DamagedPlayers?.Clear();
            DamagedPlayers = null;
        }

        public override void PostUpdatePlayers() {
            DamagedPlayers.Clear();
            for (byte i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && Main.player[i].statLife < Main.player[i].statLifeMax2) {
                    DamagedPlayers.Add(i);
                }
            }
        }
    }
}