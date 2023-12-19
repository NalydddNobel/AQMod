using Aequus.Common.Particles;
using Aequus.Content.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace Aequus.Core.Graphics;

public sealed class RenderTargetSystem : ModSystem {
    public static readonly List<RequestRenderer> RenderTargets = new();

    public override void Load() {
        On_OverlayManager.Draw += OverlayManager_Draw;
        On_Main.DoDraw_UpdateCameraPosition += Main_DoDraw_UpdateCameraPosition;
    }

    #region Hooks
    private void OverlayManager_Draw(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch) {
        if (layer == RenderLayers.ForegroundWater) {
            if (beginSpriteBatch) {
                spriteBatch.BeginWorld(shader: false);
            }

            //SpecialTileRenderer.Render(TileRenderLayer.PostDrawLiquids);
            ParticleSystem.GetLayer(ParticleLayer.AboveLiquid).Draw(spriteBatch);
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

    private static void Main_DoDraw_UpdateCameraPosition(On_Main.orig_DoDraw_UpdateCameraPosition orig) {
        orig();
        if (Main.gameMenu) {
            return;
        }

        foreach (var r in RenderTargets) {
            r.CheckSelfRequest();
            r.PrepareRenderTarget(Main.instance.GraphicsDevice, Main.spriteBatch);
        }
    }
    #endregion

    public override void Unload() {
        RenderTargets.Clear();
    }

    public override void ClearWorld() {
        foreach (var target in RenderTargets) {
            target.ClearWorld();
        }
    }
}
