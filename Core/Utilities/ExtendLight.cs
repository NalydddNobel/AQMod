using System;
using System.Runtime.CompilerServices;

namespace Aequus.Core.Utilities;

public static class ExtendLight {
    /// <param name="rgb">The RGB.</param>
    /// <returns>Gets the Brightness of a Light Vector. (The largest X,Y,Z value.)</returns>
    public static Single GetLightBrightness(Vector3 rgb) {
        return Math.Max(Math.Max(rgb.X, rgb.Y), rgb.Z);
    }

    /// <param name="rgb">The RGB.</param>
    /// <param name="lightPower">The Light Magnitude</param>
    /// <returns>Gets a Light Vector with a specified Light Magnitude. (Light Power)</returns>
    public static Vector3 ApplyLightBrightness(Vector3 rgb, Single lightPower) {
        return Vector3.Normalize(rgb) * lightPower;
    }

    /// <param name="tilePosition">The Center tile coordinates.</param>
    /// <param name="tilesSize">The size, in tiles, this is divided by 2 for some dumb reason.</param>
    /// <returns>The brightest light found within the specified area.</returns>
    public static Color GetBrightestLight(Point tilePosition, Int32 tilesSize) {
        var lighting = Color.Black;
        Int32 realSize = tilesSize / 2;
        tilePosition.WorldClamp(10 + realSize);
        for (Int32 i = tilePosition.X - realSize; i <= tilePosition.X + realSize; i++) {
            for (Int32 j = tilePosition.Y - realSize; j <= tilePosition.Y + realSize; j++) {
                var v = Lighting.GetColor(i, j);
                lighting.R = Math.Max(v.R, lighting.R);
                lighting.G = Math.Max(v.G, lighting.G);
                lighting.B = Math.Max(v.B, lighting.B);
            }
        }
        return lighting;
    }

    /// <summary>Gets the mean of light surrounding a point.</summary>
    /// <param name="x">The top left tile X</param>
    /// <param name="y">The top left tile Y</param>
    /// <param name="width">The width in tiles</param>
    /// <param name="height">The width in tiles</param>
    public static Color GetLightingSection(Int32 x, Int32 y, Int32 width, Int32 height) {
        Vector3 lighting = Vector3.Zero;
        Single amount = 0f;
        Int32 largestSide = Math.Max(width, height);
        x = Math.Clamp(x, width, Main.maxTilesX - width);
        y = Math.Clamp(y, height, Main.maxTilesY - height);
        for (Int32 i = x; i < x + width; i++) {
            for (Int32 j = y; j < y + height; j++) {
                lighting += Lighting.GetColor(i, j).ToVector3();
                amount++;
            }
        }
        return amount == 0f ? Color.White : new Color(lighting / amount);
    }
    /// <summary><inheritdoc cref="GetLightingSection(Int32, Int32, Int32, Int32)"/></summary>
    /// <param name="tilePosition">The tile top left position.</param>
    /// <param name="tilesSize">The size in tile coordinates</param>
    public static Color GetLightingSection(Point tilePosition, Int32 tilesSize) {
        return GetLightingSection(tilePosition.X, tilePosition.Y, tilesSize, tilesSize);
    }
    /// <summary><inheritdoc cref="GetLightingSection(Int32, Int32, Int32, Int32)"/></summary>
    /// <param name="x">The top left tile X</param>
    /// <param name="y">The top left tile Y</param>
    /// <param name="tilesSize">The size in tile coordinates</param>
    public static Color GetLightingSection(Int32 x, Int32 y, Int32 tilesSize) {
        return GetLightingSection(x, y, tilesSize, tilesSize);
    }

    /// <param name="worldCoordinates">The World coordinates.</param>
    /// <returns>The Light Color at a specified world coordinate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color Get(Vector2 worldCoordinates) {
        return Lighting.GetColor(worldCoordinates.ToTileCoordinates());
    }
}