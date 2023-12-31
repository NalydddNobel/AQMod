using ReLogic.Graphics;
using System.Runtime.CompilerServices;
using Terraria.UI.Chat;

namespace Aequus.Core.Utilities;

public static class SpriteBatchExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawText(this SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false) => ChatManager.DrawColorCodedString(spriteBatch, font, text, position, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawText(this SpriteBatch spriteBatch, DynamicSpriteFont font, object text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false) => spriteBatch.DrawText(font, text.ToString(), position, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BeginTiles(this SpriteBatch spriteBatch, bool shader = false) {
        spriteBatch.Begin(!shader ? SpriteSortMode.Deferred : SpriteSortMode.Immediate, null, null, null, null, null, Matrix.Identity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BeginDusts(this SpriteBatch spriteBatch, bool immediate = false, Matrix? overrideMatrix = null) {
        spriteBatch.Begin(!immediate ? SpriteSortMode.Deferred : SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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