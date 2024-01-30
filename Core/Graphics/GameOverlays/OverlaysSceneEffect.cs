using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace Aequus.Core.Graphics.GameOverlays;

public sealed class OverlaysSceneEffect : ModSceneEffect {
    public static readonly List<AequusOverlay> RegisteredOverlays = new();

    public override void Unload() {
        RegisteredOverlays.Clear();
    }

    public override void SpecialVisuals(Player player, System.Boolean isActive) {
        if (Main.myPlayer != player.whoAmI) {
            return;
        }

        foreach (var overlay in RegisteredOverlays) {
            System.Boolean inZone = overlay.SpecialVisuals(player);
            System.String biomeName = overlay.EffectKey;

            if (inZone == (Overlays.Scene[biomeName].Mode != OverlayMode.Inactive)) {
                continue;
            }

            if (inZone) {
                Overlays.Scene.Activate(biomeName);
            }
            else {
                Overlays.Scene[biomeName].Deactivate();
            }
        }
    }
}