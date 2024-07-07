namespace AequusRemake.Core.Util.Extensions;

public static class SpriteBatchExtensions {
    public static void BeginTiles(this SpriteBatch spriteBatch, bool shader = false) {
        spriteBatch.Begin(!shader ? SpriteSortMode.Deferred : SpriteSortMode.Immediate, null, null, null, null, null, Matrix.Identity);
    }

    public static void BeginUI(this SpriteBatch spriteBatch, bool immediate = false, bool useScissorRectangle = false, Matrix? overrideMatrix = null) {
        RasterizerState rasterizer = null;
        if (useScissorRectangle) {
            rasterizer = new RasterizerState {
                CullMode = CullMode.None,
                ScissorTestEnable = true
            };
        }
        spriteBatch.Begin(!immediate ? SpriteSortMode.Deferred : SpriteSortMode.Immediate, null, null, null, rasterizer, null, overrideMatrix ?? Main.UIScaleMatrix);
    }

    public static void BeginDusts(this SpriteBatch spriteBatch, bool immediate = false, Matrix? overrideMatrix = null) {
        spriteBatch.Begin(!immediate ? SpriteSortMode.Deferred : SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
    }

    public static void BeginWorld(this SpriteBatch spriteBatch, bool shader = false, Matrix? overrideMatrix = null) {
        var matrix = overrideMatrix ?? Main.Transform;
        if (!shader) {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, matrix);
        }
        else {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, matrix);
        }
    }
}