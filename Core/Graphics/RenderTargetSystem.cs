using Aequus.Common.Particles;
using Aequus.Content.Biomes.PollutedOcean.Tiles.SeaPickles;
using Aequus.Content.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace Aequus.Core.Graphics;

public sealed class RenderTargetSystem : ModSystem {
    public static readonly List<RequestRenderer> RenderTargets = new();

    public override void Load() {
        On_Main.DoDraw_UpdateCameraPosition += Main_DoDraw_UpdateCameraPosition;
    }

    #region Hooks
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
