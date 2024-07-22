using System;
using System.Collections.Generic;

namespace Aequus.Common.Structures;

public class Target {
    private static readonly List<RenderTarget2D> _loadedRenderTargets = [];

    public static RenderTarget2D NewRenderTarget(GraphicsDevice gd, int width, int height, RenderTargetUsage usage = RenderTargetUsage.DiscardContents) {
        return NewRenderTarget(new RenderTarget2D(gd, width, height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, preferredMultiSampleCount: 0, usage));
    }

    public static RenderTarget2D NewRenderTarget(RenderTarget2D target) {
        if (!_loadedRenderTargets.Contains(target)) {
            _loadedRenderTargets.Add(target);
        }

        return target;
    }

    public static bool CheckTargetCycle(ref RenderTarget2D target, int desiredWidth, int desiredHeight, GraphicsDevice device = null, RenderTargetUsage usage = RenderTargetUsage.DiscardContents) {
        if (!BadRenderTarget(target, desiredWidth, desiredHeight)) {
            return true;
        }

        device ??= Main.instance.GraphicsDevice;
        try {
            DiscardTarget(ref target);

            if (Main.IsGraphicsDeviceAvailable) {
                target = NewRenderTarget(device, desiredWidth, desiredHeight, RenderTargetUsage.PreserveContents);
            }
        }
        catch (Exception ex) {
            Aequus.Instance.Logger.Error(ex);
        }
        finally {
        }
        return false;
    }

    public static void DiscardTarget(ref RenderTarget2D target) {
        if (target == null) {
            return;
        }

        _loadedRenderTargets.Remove(target);
        if (!target.IsDisposed) {
            Main.QueueMainThreadAction(target.Dispose);
        }

        target = null;
    }

    public static bool BadRenderTarget(RenderTarget2D renderTarget2D) {
        return renderTarget2D == null || renderTarget2D.IsDisposed || renderTarget2D.IsContentLost;
    }
    public static bool BadRenderTarget(RenderTarget2D renderTarget2D, int desiredWidth, int desiredHeight) {
        return BadRenderTarget(renderTarget2D) || renderTarget2D.Width != desiredWidth || renderTarget2D.Height != desiredHeight;
    }
}
