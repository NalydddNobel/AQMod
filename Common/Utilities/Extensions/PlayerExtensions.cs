using System.Collections.Generic;

namespace Aequus.Common.Utilities.Extensions;

public static class PlayerExtensions {
    public static IEnumerable<Player> FindNearbyTeammates(this Player player, float Distance, bool AllowDead = false) {
        int team = player.team;
        for (int i = 0; i < Main.maxPlayers; i++) {
            Player other = Main.player[i];
            if (!other.active || (other.dead && !AllowDead) || other.team != team || player.Distance(other.Center) > Distance) {
                break;
            }

            yield return other;
        }
    }
}
