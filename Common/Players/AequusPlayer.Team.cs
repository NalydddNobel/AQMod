using Aequus.Core.CodeGeneration;

namespace Aequus;

public partial class AequusPlayer {
    public static float TeamBuffDistanceFalloff = 16000f;

    [ResetEffects]
    public bool infiniteWormhole;

    private void UpdateTeamEffects() {
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && Main.player[i].team == Player.team && Main.player[i].TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && Player.Distance(Main.player[i].Center) < TeamBuffDistanceFalloff) {
                UpdateGoldenFeather(Main.player[i], aequusPlayer);
            }
        }
    }

    public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer) {
        if (!otherPlayer.TryGetModPlayer<AequusPlayer>(out var otherAequusPlayer)) {
            return;
        }

        // TODO: Automate this?
        accMonocle |= otherAequusPlayer.accMonocle;
        accShimmerMonocle |= otherAequusPlayer.accShimmerMonocle;
        accDayCalendar |= otherAequusPlayer.accDayCalendar;
    }
}