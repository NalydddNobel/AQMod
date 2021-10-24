using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Common.Utilities
{
    internal struct Projector3D
    {
        public const float Z_VIEW = -20f;

        public readonly Vector2 WorldViewPosition;
        public readonly float ViewScale;

        internal static Vector2 GetParralaxPosition(Vector2 origin, float z)
        {
            z = MultZ(z);
            var viewPos = new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f);
            return new Vector2(origin.X - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.X - viewPos.X), origin.Y - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.Y - viewPos.Y));
        }

        public static float GetParralaxScale(float originalScale, float z)
        {
            z = MultZ(z);
            return originalScale * (-Z_VIEW / (z - Z_VIEW));
        }

        public static float MultZ(float z)
        {
            return z *= AQMod.Effect3Dness + 0.01f; // adding 0.001 because some things actually rely on z for layering
        }

        /// <summary>
        /// Automatically parralax lerps Z
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="baseScale"></param>
        /// <param name="z"></param>
        public Projector3D(Vector2 worldPosition, float baseScale, float z)
        {
            z = MultZ(z);
            WorldViewPosition = GetParralaxPosition(worldPosition, z);
            ViewScale = GetParralaxScale(baseScale, z);
        }
    }
}