using System;
using System.Collections.Generic;

namespace Aequus.Core.Graphics;

public sealed class RenderTargetRequests : ILoad {
    private readonly List<IRenderTargetRequest> _requests = new(4);

    public static RenderTargetRequests Instance { get; private set; }

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
        RenderTargetBinding[] oldRenderTargets = g?.GetRenderTargets();
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
            g?.SetRenderTargets(oldRenderTargets);
        }
    }

    public interface IRenderTargetRequest : IDisposable {
        bool IsDisposed { get; set; }
        bool Active { get; set; }
        Point Dimensions { get; }
        RenderTarget2D RenderTarget { get; set; }

        void DrawOntoRenderTarget(GraphicsDevice graphics, SpriteBatch sb);
    }

    public void Load(Mod mod) {
        Instance = this;
        Main.OnPreDraw += HandleRenderTargetRequests;
    }


    public void Unload() {
        Main.OnPreDraw -= HandleRenderTargetRequests;
        Instance = null;
    }
}
