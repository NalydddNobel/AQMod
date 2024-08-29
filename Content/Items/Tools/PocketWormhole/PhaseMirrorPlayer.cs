using System.Collections.Generic;

namespace Aequus.Content.Items.Tools.PocketWormhole;

internal class PhaseMirrorPlayer : ModPlayer {
    public readonly List<Dust> DustCache = [];

    public Rectangle GetAnimationBox() {
        var hitbox = Player.getRect();
        hitbox.Inflate(12, 12);
        hitbox.Width += 1;
        hitbox.Height -= 6;
        return hitbox;
    }

    public override void PreUpdate() {
        if (DustCache.Count <= 0) {
            return;
        }

        if (!Player.ItemAnimationActive) {
            DustCache.Clear();
            return;
        }

        Rectangle hitbox = GetAnimationBox();
        for (int i = 0; i < DustCache.Count; i++) {
            Dust d = DustCache[i];
            if (!d.active) {
                DustCache.RemoveAt(i);
                i--;
            }
            else if (d.customData is PhaseMirrorDustInstanceInfo info) {
                d.position = hitbox.TopLeft() + new Vector2(info.X, info.Y);
            }
        }
    }
}
