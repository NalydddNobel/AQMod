namespace Aequus.Common.Utilities;

public static class ExtendRenderTarget {
    public static void CheckRenderTarget2D(ref RenderTarget2D renderTarget, int wantedWidth, int wantedHeight, RenderTargetUsage usage = RenderTargetUsage.DiscardContents) {
        if (renderTarget == null || renderTarget.IsContentLost || renderTarget.IsDisposed
            || renderTarget.Width != wantedWidth || renderTarget.Height != wantedHeight) {

            if (renderTarget != null && !renderTarget.IsDisposed) {
                renderTarget.Dispose();
            }

            renderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, wantedWidth, wantedHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None, preferredMultiSampleCount: 0, usage);
        }
    }
}
