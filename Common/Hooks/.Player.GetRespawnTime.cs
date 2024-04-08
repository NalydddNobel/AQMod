using System;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static int On_Player_GetRespawnTime(On_Player.orig_GetRespawnTime orig, Player player, bool pvp) {
        int time = orig(player, pvp);
        if (time <= AequusPlayer.MinimumRespawnTime || !player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return time;
        }
        return Math.Max(time + aequusPlayer.respawnTimeModifierFlat, AequusPlayer.MinimumRespawnTime);
    }
}
