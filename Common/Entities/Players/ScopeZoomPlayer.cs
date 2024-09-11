using System;

namespace Aequus.Common.Entities.Players;

public class ScopeZoomPlayer : ModPlayer {
    public override void ModifyZoom(ref float zoom) {
        if (Player.HeldItem.ModItem is IScopeWhileHeld heldScope) {
            zoom = Math.Max(heldScope.GetZoomFactor(Player), zoom);
        }
    }
}

public interface IScopeWhileHeld {
    float GetZoomFactor(Player player);
}
