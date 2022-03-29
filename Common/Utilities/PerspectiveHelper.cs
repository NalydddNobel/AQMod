using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Common.Utilities
{
    public static class PerspectiveHelper
    {
        public const float Z_VIEW = -20f;

        internal static Vector2 GetParralaxPosition(Vector2 origin, float z)
        {
            return GetParralaxPosition(origin, z, new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f));
        }

        internal static Vector2 GetParralaxPosition(Vector2 origin, float z, Vector2 viewPos)
        {
            return new Vector2(origin.X - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.X - viewPos.X), origin.Y - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.Y - viewPos.Y));
        }

        public static float GetParralaxScale(float originalScale, float z)
        {
            return originalScale * (-Z_VIEW / (z - Z_VIEW));
        }
    }
}
