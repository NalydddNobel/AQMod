using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus;

public static class DrawHelper {
    public delegate void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);

    public static Vector2 TileDrawOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

    public static void DrawLine(Draw draw, Vector2 start, float rotation, float length, float width, Color color) {
        draw(TextureAssets.MagicPixel.Value, start, new Rectangle(0, 0, 1, 1), color, rotation, new Vector2(1f, 0.5f), new Vector2(length, width), SpriteEffects.None, 0f);
    }
    public static void DrawLine(Vector2 start, float rotation, float length, float width, Color color) {
        DrawLine(Main.spriteBatch.Draw, start, rotation, length, width, color);
    }
    public static void DrawLine(Draw draw, Vector2 start, Vector2 end, float width, Color color) {
        DrawLine(draw, start, (start - end).ToRotation(), (end - start).Length(), width, color);
    }
    public static void DrawLine(Vector2 start, Vector2 end, float width, Color color) {
        DrawLine(Main.spriteBatch.Draw, start, end, width, color);
    }

    #region Shaders
    public static int ShaderColorOnlyIndex => ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
    public static ArmorShaderData ShaderColorOnly => GameShaders.Armor.GetSecondaryShader(ShaderColorOnlyIndex, Main.LocalPlayer);
    #endregion

    #region Colors
    public static Color GetStringColor(int stringColorID) {
        if (stringColorID == 27) {
            return Main.DiscoColor;
        }
        return WorldGen.paintColor(stringColorID);
    }
    #endregion

    #region Lighting
    public static Color GetBrightestLight(Point tilePosition, int tilesSize) {
        var lighting = Color.Black;
        int realSize = tilesSize / 2;
        tilePosition.WorldClamp(10 + realSize);
        for (int i = tilePosition.X - realSize; i <= tilePosition.X + realSize; i++) {
            for (int j = tilePosition.Y - realSize; j <= tilePosition.Y + realSize; j++) {
                var v = Lighting.GetColor(i, j);
                lighting.R = Math.Max(v.R, lighting.R);
                lighting.G = Math.Max(v.G, lighting.G);
                lighting.B = Math.Max(v.B, lighting.B);
            }
        }
        return lighting;
    }

    /// <summary>
    /// Gets the mean of light surrounding a point
    /// </summary>
    /// <param name="x">The tile's X coordinate</param>
    /// <param name="y">The tile's Y coordinate</param>
    /// <param name="width">The width in tiles</param>
    /// <param name="height">The width in tiles</param>
    /// <returns></returns>
    public static Color GetLightingSection(int x, int y, int width, int height) {
        Vector3 lighting = Vector3.Zero;
        float amount = 0f;
        int largestSide = Math.Max(width, height);
        x = Math.Clamp(x, largestSide, Main.maxTilesX - largestSide);
        y = Math.Clamp(y, largestSide, Main.maxTilesY - largestSide);
        for (int i = x; i < x + width; i++) {
            for (int j = y; j < y + height; j++) {
                lighting += Lighting.GetColor(i, j).ToVector3();
                amount++;
            }
        }
        if (amount == 0f)
            return Color.White;
        return new Color(lighting / amount);
    }
    /// <summary>
    /// Gets the mean of light surrounding a point
    /// </summary>
    /// <param name="tilePosition">The tile center</param>
    /// <param name="width">The width in tiles</param>
    /// <param name="height">The width in tiles</param>
    /// <returns></returns>
    public static Color GetLightingSection(Point tilePosition, int width, int height) {
        return GetLightingSection(tilePosition.X - width, tilePosition.Y, width, height);
    }
    /// <summary>
    /// Gets the mean of light surrounding a point
    /// </summary>
    /// <param name="tilePosition">The tile center</param>
    /// <param name="tilesSize">The size in tile coordinates</param>
    /// <returns></returns>
    public static Color GetLightingSection(Point tilePosition, int tilesSize = 10) {
        Vector3 lighting = Vector3.Zero;
        float amount = 0f;
        int realSize = tilesSize / 2;
        tilePosition.WorldClamp(10 + realSize);
        for (int i = tilePosition.X - realSize; i <= tilePosition.X + realSize; i++) {
            for (int j = tilePosition.Y - realSize; j <= tilePosition.Y + realSize; j++) {
                lighting += Lighting.GetColor(i, j).ToVector3();
                amount++;
            }
        }
        if (amount == 0f)
            return Color.White;
        return new Color(lighting / amount);
    }
    /// <summary>
    /// Gets the mean of light surrounding a point
    /// </summary>
    /// <param name="x">The center tile X</param>
    /// <param name="y">The center tile Y</param>
    /// <param name="tilesSize">The size in tile coordinates</param>
    /// <returns></returns>
    public static Color GetLightingSection(int x, int y, int tilesSize = 10) {
        return GetLightingSection(new Point(x, y), tilesSize);
    }
    /// <summary>
    /// Gets the mean of light surrounding a point
    /// </summary>
    /// <param name="worldPosition">The center</param>
    /// <param name="tilesSize">The size in tile coordinates</param>
    /// <returns></returns>
    public static Color GetLightingSection(Vector2 worldPosition, int tilesSize = 10) {
        return GetLightingSection(worldPosition.ToTileCoordinates(), tilesSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color GetLightColor(Vector2 worldCoordinates) {
        return Lighting.GetColor(worldCoordinates.ToTileCoordinates());
    }
    #endregion

    #region spriteBatch.Begin
    public static void Begin_Tiles(this SpriteBatch spriteBatch, bool shader = false) {
        spriteBatch.Begin(!shader ? SpriteSortMode.Deferred : SpriteSortMode.Immediate, null, null, null, null, null, Matrix.Identity);
    }

    public static void Begin_UI(this SpriteBatch spriteBatch, bool immediate = false, bool useScissorRectangle = false, Matrix? matrix = null) {
        RasterizerState rasterizer = null;
        if (useScissorRectangle) {
            rasterizer = new RasterizerState {
                CullMode = CullMode.None,
                ScissorTestEnable = true
            };
        }
        spriteBatch.Begin(!immediate ? SpriteSortMode.Deferred : SpriteSortMode.Immediate, null, null, null, rasterizer, null, matrix ?? Main.UIScaleMatrix);
    }

    public static void Begin_Dusts(this SpriteBatch spriteBatch, bool immediate = false, Matrix? overrideMatrix = null) {
        spriteBatch.Begin(!immediate ? SpriteSortMode.Deferred : SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
    }

    public static void Begin_World(this SpriteBatch spriteBatch, bool shader = false, Matrix? overrideMatrix = null) {
        var matrix = overrideMatrix ?? Main.Transform;
        if (!shader) {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, matrix);
        }
        else {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, matrix);
        }
    }
    #endregion
}