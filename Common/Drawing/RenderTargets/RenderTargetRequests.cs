using Aequus.Common;
using Aequus.Common.Utilities;
using System;
using System.Collections.Generic;

namespace Aequus.Common.Graphics;

public class RenderTargetRequests : LoadedType {
    private readonly List<IRenderTargetRequest> _requests = new(4);

    public void Request(IRenderTargetRequest request) {
        if (request.Active) {
            return;
        }

        request.Active = true;
        request.IsDisposed = false;
        _requests.Add(request);
    }

    private void HandleRenderTargetRequests(GameTime obj) {
        if (_requests.Count == 0) {
            return;
        }

        if (!Main.IsGraphicsDeviceAvailable) {
            return;
        }

        GraphicsDevice g = Main.instance.GraphicsDevice;
        DrawHelper.gdHelper.EnqueueRenderTargetBindings(g);
        try {
            SpriteBatch sb = Main.spriteBatch;
            for (int i = 0; i < _requests.Count; i++) {
                IRenderTargetRequest request = _requests[i];
                if (!request.Active || request.IsDisposed) {
                    if (!request.IsDisposed) {
                        request.Dispose();
                    }

                    request.Active = false;
                    _requests.RemoveAt(i);
                    continue;
                }

                request.RenderTarget ??= new RenderTarget2D(Main.instance.GraphicsDevice, request.Dimensions.X, request.Dimensions.Y, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
                g.SetRenderTarget(request.RenderTarget);
                request.DrawOntoRenderTarget(g, sb);
                g.SetRenderTarget(null);
            }
        }
        catch {
        }
        finally {
            DrawHelper.gdHelper.DequeueRenderTargetBindings(g);
        }
    }

    public interface IRenderTargetRequest : IDisposable {
        bool IsDisposed { get; set; }
        bool Active { get; set; }
        Point Dimensions { get; }
        RenderTarget2D RenderTarget { get; set; }

        void DrawOntoRenderTarget(GraphicsDevice graphics, SpriteBatch sb);
    }

    protected override void Load() {
        Main.OnPreDraw += HandleRenderTargetRequests;
    }

    protected override void Unload() {
        Main.OnPreDraw -= HandleRenderTargetRequests;
    }
}
