using Aequus.Core.Generator;

namespace Aequus;

public partial class AequusPlayer {
    public static System.Single TeamBuffDistanceFalloff = 16000f;

    [ResetEffects]
    public System.Boolean infiniteWormhole;

    private static void Player_TakeUnityPotion(On_Player.orig_TakeUnityPotion orig, Player player) {
        if (player.GetModPlayer<AequusPlayer>().infiniteWormhole) {
            return;
        }

        orig(player);
    }

    private static System.Boolean Player_HasUnityPotion(On_Player.orig_HasUnityPotion orig, Player player) {
        return player.GetModPlayer<AequusPlayer>().infiniteWormhole ? true : orig(player);
    }

    private void UpdateTeamEffects() {
        for (System.Int32 i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && Main.player[i].team == Player.team && Main.player[i].TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && Player.Distance(Main.player[i].Center) < TeamBuffDistanceFalloff) {
                PostUpdateEquips_TeamEffects_GoldenFeather(Main.player[i], aequusPlayer);
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