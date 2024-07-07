using System;

namespace AequusRemake.Core.Util.Helpers;

public sealed class ParallaxHelper {
    public const float AnchorZ = -20f;

    public static Vector2 GetViewPoint(Vector2 origin, float z) {
        return GetViewPoint(origin, z, new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f));
    }
    public static Vector2 GetViewPoint(Vector2 origin, float z, Vector2 viewPos) {
        return new Vector2(origin.X - (1f - (-AnchorZ / (z - AnchorZ))) * (origin.X - viewPos.X), origin.Y - (1f - (-AnchorZ / (z - AnchorZ))) * (origin.Y - viewPos.Y));
    }

    public static float GetViewScale(float originalScale, float z) {
        return originalScale * (-AnchorZ / (Math.Max(z, -10f) - AnchorZ));
    }
}
