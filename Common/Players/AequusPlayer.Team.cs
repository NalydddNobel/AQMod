using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    private void UpdateTeamEffects() {
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && Main.player[i].team == Player.team && Main.player[i].TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
                PostUpdateEquips_TeamEffects_GoldenFeather(Main.player[i], aequusPlayer);
            }
        }
    }
}