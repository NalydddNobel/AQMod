using System;
using System.Runtime.CompilerServices;
using Terraria.Graphics.CameraModifiers;

namespace Aequus.Core.Utilities;

public static class ViewHelper {
    public static Vector2 ScaledMouseScreen => Vector2.Transform(Main.ReverseGravitySupport(Main.MouseScreen, 0f), Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
    
    public static Vector2 ScaledMouseworld => ScaledMouseScreen + Main.screenPosition;

    public const float Z_VIEW = -20f;

    public static Vector2 ScreenCenter {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            return Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
        }
    }

    [Obsolete("Replace with non-legacy screen shake", Aequus.DEBUG_MODE)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LegacyScreenShake(float intensity, float multiplyPerTick = 0.9f, Vector2 where = default) {
#if !DEBUG
        if (where != default(Vector2)) {
            float distance = Vector2.Distance(where, Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f)) / 1200f;
            if (float.IsNaN(distance) || distance >= 1f) {
                return;
            }

            intensity *= 1f - distance;
        }

        if (Main.netMode != NetmodeID.Server) {
            Main.instance.CameraModifiers.Add(new Old.Common.Graphics.Camera.LegacyScreenShakeModifier(intensity, multiplyPerTick));
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PunchCameraTo(Vector2 position, float strength = 2f, float vibrationsPerSecond = 4f, int frames = 10) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        Vector2 difference = ScreenCenter - position;
        float distance = difference.Length();
        float intensity = 1f - distance / 150f / strength;
        if (intensity > 0f) {
            PunchCamera(Vector2.Normalize(difference), strength * intensity, vibrationsPerSecond, frames);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PunchCameraScalingFrom(Vector2 direction, Vector2 position, float strength = 2f, float vibrationsPerSecond = 4f, int frames = 10) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        Vector2 difference = ScreenCenter - position;
        float distance = difference.Length();
        float intensity = 1f - distance / 150f / strength;
        PunchCamera(direction, strength * intensity, vibrationsPerSecond, frames);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PunchCamera(Vector2 direction, float strength = 2f, float vibrationsPerSecond = 4f, int frames = 10) {
        if (Main.netMode != NetmodeID.Server) {
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Vector2.Zero, direction, strength, vibrationsPerSecond, frames));
        }
    }

    public static Vector2 GetViewPoint(Vector2 origin, float z) {
        return GetViewPoint(origin, z, new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f));
    }
    public static Vector2 GetViewPoint(Vector2 origin, float z, Vector2 viewPos) {
        return new Vector2(origin.X - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.X - viewPos.X), origin.Y - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.Y - viewPos.Y));
    }

    public static float GetViewScale(float originalScale, float z) {
        return originalScale * (-Z_VIEW / (Math.Max(z, -10f) - Z_VIEW));
    }
}
