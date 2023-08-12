using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.Common.Utilities {
    public static class ViewHelper {
        public const float Z_VIEW = -20f;

        internal static Vector2 GetViewPoint(Vector2 origin, float z) {
            return GetViewPoint(origin, z, new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f));
        }
        internal static Vector2 GetViewPoint(Vector2 origin, float z, Vector2 viewPos) {
            return new Vector2(origin.X - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.X - viewPos.X), origin.Y - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.Y - viewPos.Y));
        }

        public static float GetViewScale(float originalScale, float z) {
            return originalScale * (-Z_VIEW / (Math.Max(z, -10f) - Z_VIEW));
        }
    }
}
