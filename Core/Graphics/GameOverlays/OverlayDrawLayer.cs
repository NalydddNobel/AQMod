using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace AequusRemake.Core.Graphics.GameOverlays;

public abstract class OverlayDrawLayer(EffectPriority priority, RenderLayers layer) : AequusRemakeOverlay(priority, layer) {
    public readonly List<IOverlayDrawer> Drawers = [];

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
    }

    public override bool SpecialVisuals(Player player) {
        return Drawers.Count > 0;
    }

    public override void Draw(SpriteBatch spriteBatch) {
        foreach (var anim in Drawers) {
            anim.Draw(spriteBatch);
        }
    }
}