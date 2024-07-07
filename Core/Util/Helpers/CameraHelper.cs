using System.Runtime.CompilerServices;
using Terraria.Graphics.CameraModifiers;

namespace AequusRemake.Core.Util.Helpers;
internal class CameraHelper {
    public static Vector2 ScaledMouseScreen => Vector2.Transform(Main.ReverseGravitySupport(Main.MouseScreen, 0f), Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));

    public static Vector2 ScaledMouseworld => ScaledMouseScreen + Main.screenPosition;

    public static Vector2 Center {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PunchTowards(Vector2 position, float strength = 2f, float vibrationsPerSecond = 4f, int frames = 10) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        Vector2 difference = Center - position;
        float distance = difference.Length();
        float intensity = 1f - distance / 150f / strength;
        if (intensity > 0f) {
            Punch(Vector2.Normalize(difference), strength * intensity, vibrationsPerSecond, frames);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PunchFrom(Vector2 direction, Vector2 position, float strength = 2f, float vibrationsPerSecond = 4f, int frames = 10) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        Vector2 difference = Center - position;
        float distance = difference.Length();
        float intensity = 1f - distance / 150f / strength;
        Punch(direction, strength * intensity, vibrationsPerSecond, frames);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Punch(Vector2 direction, float strength = 2f, float vibrationsPerSecond = 4f, int frames = 10) {
        if (Main.netMode != NetmodeID.Server) {
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Vector2.Zero, direction, strength, vibrationsPerSecond, frames));
        }
    }
}
