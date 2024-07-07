namespace AequusRemake.Core.Util.Extensions;

public static class RenderTargetExtensions {
    public static void Check(ref RenderTarget2D renderTarget, int wantedWidth, int wantedHeight, RenderTargetUsage usage = RenderTargetUsage.DiscardContents) {
        if (renderTarget == null || renderTarget.IsContentLost || renderTarget.IsDisposed
            || renderTarget.Width != wantedWidth || renderTarget.Height != wantedHeight) {

            if (renderTarget != null && !renderTarget.IsDisposed) {
                renderTarget.Dispose();
            }

            renderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, wantedWidth, wantedHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None, preferredMultiSampleCount: 0, usage);
        }
    }
}
