using System;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    /// <summary>
    /// The lowest any respawn-time reducing items can go.
    /// </summary>
    public static int MinimumRespawnTime = 180;

    public int respawnTimeModifier;

    private static int On_Player_GetRespawnTime(On_Player.orig_GetRespawnTime orig, Player player, bool pvp) {
        int time = orig(player, pvp);
        if (time <= MinimumRespawnTime || !player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return time;
        }
        return Math.Max(time + aequusPlayer.respawnTimeModifier, MinimumRespawnTime);
    }
}