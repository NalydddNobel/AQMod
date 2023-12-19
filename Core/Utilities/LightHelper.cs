using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Aequus.Core.Utilities;

public static class LightHelper {
    public static readonly Vector3 ShadowOrb = new Vector3(0.6f, 0.1f, 1f);
    public static readonly Vector3 CrimsonHeart = new Vector3(1f, 0.11f, 0.33f);
    public static readonly Vector3 BlueFairy = new Vector3(0.45f, 0.75f, 1f);
    public static readonly Vector3 PinkFairy = new Vector3(1f, 0.45f, 0.75f);
    public static readonly Vector3 GreenFairy = new Vector3(0.45f, 1f, 0.75f);
    public static readonly Vector3 Flickerwick = new Vector3(0.3f, 0.5f, 1f);
    public static readonly Vector3 WispInABottle = new Vector3(0.5f, 0.9f, 1f);
    public static readonly Vector3 SuspiciousLookingTentacle = new Vector3(0.5f, 0.9f, 1f);
    public static readonly Vector3 PumpkinScentedCandle = new Vector3(1f, 0.7f, 0.05f);
    public static readonly Vector3 ToyGolem = new Vector3(1f, 0.61f, 0.16f);
    public static readonly Vector3 FairyPrincess = new Vector3(1f, 0.6f, 1f);

    public static Vector3 OmegaStarite { get; set; } = new Vector3(0.1f, 0.6f, 1f);

    public const float ShadowOrbBrightness = 0.9f;
    public const float FairyBrightness = 0.9f;
    public const float FlickerwickBrightness = 1f;
    public const float WispInABottleBrightness = 1.5f;
    public const float SuspiciousLookingTentacleBrightness = 2f;
    public const float PumpkinScentedCandleBrightness = 1.5f;
    public const float ToyGolemBrightness = 1.5f;
    public const float FairyPrincessBrightness = 1.5f;

    public static float OmegaStariteBrightness { get; set; } = 1f;

    public static float GetLightBrightness(Vector3 rgb) {
        return Math.Max(Math.Max(rgb.X, rgb.Y), rgb.Z);
    }

    public static Vector3 ApplyLightBrightness(Vector3 rgb, float lightPower) {
        return Vector3.Normalize(rgb) * lightPower;
    }

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
}