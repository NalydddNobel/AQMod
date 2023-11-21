﻿using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Aequus.Core.Graphics.GameOverlays;

public sealed class OverlaysSceneEffect : ModSceneEffect {
    public static readonly List<AequusOverlay> RegisteredOverlays = new();

    public override void Unload() {
        RegisteredOverlays.Clear();
    }

    public override void SpecialVisuals(Player player, bool isActive) {
        if (Main.myPlayer != player.whoAmI) {
            return;
        }

        foreach (var overlay in RegisteredOverlays) {
            bool inZone = overlay.SpecialVisuals(player);
            string biomeName = overlay.EffectKey;

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