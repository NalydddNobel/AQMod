using System;
using System.Runtime.CompilerServices;

namespace Aequu2.Core.Utilities;

public static class ExtendLight {
    /// <param name="rgb">The RGB.</param>
    /// <returns>Gets the Brightness of a Light Vector. (The largest X,Y,Z value.)</returns>
    public static float GetLightBrightness(Vector3 rgb) {
        return Math.Max(Math.Max(rgb.X, rgb.Y), rgb.Z);
    }

    /// <param name="rgb">The RGB.</param>
    /// <param name="lightPower">The Light Magnitude</param>
    /// <returns>Gets a Light Vector with a specified Light Magnitude. (Light Power)</returns>
    public static Vector3 ApplyLightBrightness(Vector3 rgb, float lightPower) {
        return Vector3.Normalize(rgb) * lightPower;
    }

    /// <param name="tilePosition">The Center tile coordinates.</param>
    /// <param name="tilesSize">The size, in tiles, this is divided by 2 for some dumb reason.</param>
    /// <returns>The brightest light found within the specified area.</returns>
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

    /// <summary>Gets the mean of light surrounding a point.</summary>
    /// <param name="x">The top left tile X</param>
    /// <param name="y">The top left tile Y</param>
    /// <param name="width">The width in tiles</param>
    /// <param name="height">The width in tiles</param>
    public static Color GetLightingSection(int x, int y, int width, int height) {
        Vector3 lighting = Vector3.Zero;
        float amount = 0f;
        int largestSide = Math.Max(width, height);
        x = Math.Clamp(x, width, Main.maxTilesX - width);
        y = Math.Clamp(y, height, Main.maxTilesY - height);
        for (int i = x; i < x + width; i++) {
            for (int j = y; j < y + height; j++) {
                lighting += Lighting.GetColor(i, j).ToVector3();
                amount++;
            }
        }
        return amount == 0f ? Color.White : new Color(lighting / amount);
    }
    /// <summary><inheritdoc cref="GetLightingSection(int, int, int, int)"/></summary>
    /// <param name="tilePosition">The tile top left position.</param>
    /// <param name="tilesSize">The size in tile coordinates</param>
    public static Color GetLightingSection(Point tilePosition, int tilesSize) {
        return GetLightingSection(tilePosition.X, tilePosition.Y, tilesSize, tilesSize);
    }
    /// <summary><inheritdoc cref="GetLightingSection(int, int, int, int)"/></summary>
    /// <param name="x">The top left tile X</param>
    /// <param name="y">The top left tile Y</param>
    /// <param name="tilesSize">The size in tile coordinates</param>
    public static Color GetLightingSection(int x, int y, int tilesSize) {
        return GetLightingSection(x, y, tilesSize, tilesSize);
    }

    /// <param name="worldCoordinates">The World coordinates.</param>
    /// <returns>The Light Color at a specified world coordinate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color Get(Vector2 worldCoordinates) {
        return Lighting.GetColor(worldCoordinates.ToTileCoordinates());
    }

    /// <param name="worldCoordinates">The World coordinates.</param>
    /// <returns>The Light Color at a specified world coordinate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color Get(Vector2 worldCoordinates, Color originalColor) {
        return Lighting.GetColor(worldCoordinates.ToTileCoordinates(), originalColor);
    }
}