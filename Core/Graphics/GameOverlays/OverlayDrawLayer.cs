using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace Aequus.Core.Graphics.GameOverlays;

public abstract class OverlayDrawLayer : AequusOverlay {
    public readonly List<IOverlayDrawer> Draws = new();

    protected OverlayDrawLayer(EffectPriority priority, RenderLayers layer) : base(priority, layer) {
    }

    public void Add(IOverlayDrawer anim) {
        Draws.Add(anim);
    }

    public System.Boolean Remove(IOverlayDrawer anim) {
        return Draws.Remove(anim);
    }

    public void Clear() {
        Draws.Clear();
    }

    public void Update() {
        for (System.Int32 i = 0; i < Draws.Count; i++) {
            if (!Draws[i].Update()) {
                Draws.RemoveAt(i);
                i--;
            }
        }
    }

    public override System.Boolean SpecialVisuals(Player player) {
        Update();
        return Draws.Count > 0;
    }

    public override void Draw(SpriteBatch spriteBatch) {
        foreach (var anim in Draws) {
            anim.Draw(spriteBatch);
        }
    }
}