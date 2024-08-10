using Aequus.Common.Drawing;
using Aequus.Common.Structures;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Tiles;

// Unfinished.

public class ReflectiveBlock {
}

public class ReflectiveEffect : RequestHandler<ReflectiveSurfaceRequest> {
    public bool Ready { get; private set; }

    private RenderTarget2D _target;

    protected override void OnActivate() {
        DrawLayers.Instance.PostUpdateScreenPosition += HandleRequestsOnPreDraw;
        DrawLayers.Instance.PostDrawLiquids += DrawTargetOntoScreen;
    }

    protected override void OnDeactivate() {
        Ready = false;
        DrawLayers.Instance.PostUpdateScreenPosition -= HandleRequestsOnPreDraw;
        DrawLayers.Instance.PostDrawLiquids -= DrawTargetOntoScreen;
    }

    private void HandleRequestsOnPreDraw(SpriteBatch sb) {
        HandleRequests();
        ClearQueue();
    }

    protected override bool HandleRequests(IEnumerable<ReflectiveSurfaceRequest> todo) {
        if (Main.instance.tileTarget == null || Main.instance.tileTarget.IsDisposed || Main.instance.tileTarget.IsContentLost) {
            return false;
        }

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice device = Main.instance.GraphicsDevice;

        if (!Target.CheckTargetCycle(ref _target, Main.instance.tileTarget.Width, Main.instance.tileTarget.Height, device, RenderTargetUsage.PreserveContents)) {
            return false;
        }

        RenderTargetBinding[] oldTargets = device.GetRenderTargets();

        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        try {
            DrawRequests(spriteBatch, todo);
        }
        catch (Exception ex) {
            Mod.Logger.Error(ex);
        }
        finally {
            spriteBatch.End();
        }

        device.SetRenderTargets(oldTargets);

        return true;
    }

    private void DrawRequests(SpriteBatch spriteBatch, IEnumerable<ReflectiveSurfaceRequest> todo) {
        Ready = false;
        if (Main.drawToScreen || !Lighting.NotRetro || !Aequus.HQ) {
            return;
        }

        foreach (ReflectiveSurfaceRequest request in todo) {

        }

        Ready = true;
    }

    private void DrawTargetOntoScreen(SpriteBatch spriteBatch) {
        if (_target == null || !Ready) {
            return;
        }
    }
}

public readonly record struct ReflectiveSurfaceRequest();
