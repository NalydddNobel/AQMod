using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace Aequus.Core.Graphics.GameOverlays;

public abstract class OverlayDrawLayer : AequusOverlay {
    public readonly List<IOverlayDrawer> Drawers = new();
    public readonly DrawCommandHandler DrawCommands = new();

    protected OverlayDrawLayer(EffectPriority priority, RenderLayers layer) : base(priority, layer) {
    }

    public void Add(IOverlayDrawer anim) {
        Drawers.Add(anim);
    }

    public bool Remove(IOverlayDrawer anim) {
        return Drawers.Remove(anim);
    }

    public void Clear() {
        Drawers.Clear();
    }

    public override void Update(GameTime gameTime) {
        for (int i = 0; i < Drawers.Count; i++) {
            if (!Drawers[i].Update()) {
                Drawers.RemoveAt(i);
                i--;
            }
        }
        DrawCommands.Clear();
    }

    public override bool SpecialVisuals(Player player) {
        return Drawers.Count > 0 || DrawCommands.Count > 0;
    }

    public override void Draw(SpriteBatch spriteBatch) {
        foreach (var anim in Drawers) {
            anim.Draw(spriteBatch);
        }
        DrawCommands.InvokeAll(spriteBatch);
    }
}