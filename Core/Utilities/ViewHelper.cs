using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Aequus;

public static class ViewHelper {
    public const float Z_VIEW = -20f;

    public static Vector2 ScreenCenter {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            return Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PunchCameraTo(Vector2 position, float strength = 2f, float vibrationsPerSecond = 4f, int frames = 10) {
        if (Main.netMode != NetmodeID.Server) {
            PunchCamera(Vector2.Normalize(position - ScreenCenter), strength, vibrationsPerSecond, frames);
        }
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
