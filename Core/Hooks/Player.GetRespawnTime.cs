using System;

namespace Aequus.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for respawn time modifications to be perfomed. (<see cref="AequusPlayer.respawnTimeModifierFlat"/>)</summary>
    private static int On_Player_GetRespawnTime(On_Player.orig_GetRespawnTime orig, Player player, bool pvp) {
        int time = orig(player, pvp);
        if (time <= AequusPlayer.MinimumRespawnTime || !player.TryGetModPlayer(out AequusPlayer aequusPlayer)) {
            return time;
        }

        return Math.Max(time + aequusPlayer.respawnTimeModifierFlat, AequusPlayer.MinimumRespawnTime);
    }
}
