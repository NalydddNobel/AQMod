using Aequus.Common.Particles;
using Aequus.Content.Graphics;
using System;
using Terraria.Graphics.Effects;

namespace Aequus.Core.Graphics;

public class DrawLayers : ModSystem {
    public static Action<SpriteBatch> PostDrawLiquids { get; set; }

    public override void Load() {
        On_OverlayManager.Draw += OverlayManager_Draw;
    }

    public override void Unload() {
        PostDrawLiquids = null;
    }

    #region Hooks
    private static void OverlayManager_Draw(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch) {
        if (layer == RenderLayers.ForegroundWater) {
            if (beginSpriteBatch) {
                spriteBatch.BeginWorld(shader: false);
            }

            ParticleSystem.GetLayer(ParticleLayer.AboveLiquid).Draw(spriteBatch);
            PostDrawLiquids?.Invoke(spriteBatch);
            if (RadonMossFogRenderer.Instance.IsReady) {
                spriteBatch.End();
                RadonMossFogRenderer.Instance.DrawOntoScreen(spriteBatch);
                spriteBatch.BeginWorld(shader: false);
            }

            if (beginSpriteBatch) {
                spriteBatch.End();
            }
        }
        orig(self, spriteBatch, layer, beginSpriteBatch);
    }
    #endregion
}