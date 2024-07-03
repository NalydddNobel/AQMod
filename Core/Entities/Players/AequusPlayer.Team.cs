using Aequu2.Core.CodeGeneration;

namespace Aequu2;

public partial class AequusPlayer {
    public static float TeamBuffDistanceFalloff = 16000f;

    [ResetEffects]
    public bool infiniteWormhole;

    private void UpdateTeamEffects() {
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && Main.player[i].team == Player.team && Main.player[i].TryGetModPlayer<AequusPlayer>(out var Aequu2Player) && Player.Distance(Main.player[i].Center) < TeamBuffDistanceFalloff) {
                UpdateGoldenFeather(Main.player[i], Aequu2Player);
            }
        }
    }
}