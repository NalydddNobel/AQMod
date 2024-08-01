using System.Collections.Generic;

namespace Aequus.Common.Utilities.Helpers;

public sealed class Active : ModSystem {
    public static Active Instance => ModContent.GetInstance<Active>();

    public readonly List<Player> Players = new();

    public override void PostUpdatePlayers() {
        Players.Clear();

        for (int i = 0; i < Main.maxPlayers; i++) {
            Player player = Main.player[i];
            if (player.active) {
                Players.Add(player);
            }
        }
    }

    public override void OnWorldLoad() {
        Players.Clear();
    }
}
