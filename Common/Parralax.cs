using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Common
{
    internal struct Parralax
    {
        public const float Z_VIEW = -20f;

        public readonly Vector2 WorldViewPosition;
        public readonly float ViewScale;

        public static float GameParallax { get; private set; }

        internal static Vector2 GetParralaxPosition(Vector2 origin, float z)
        {
            Vector2 viewPos = new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f);
            return new Vector2(origin.X - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.X - viewPos.X), origin.Y - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.Y - viewPos.Y));
        }

        public static float GetParralaxScale(float originalScale, float z) => originalScale * (-Z_VIEW / (z - Z_VIEW));

        public static float ParralaxLerp(float z)
        {
            return MathHelper.Lerp(z, 0f, GameParallax);
        }

        /// <summary>
        /// Automatically parralax lerps Z
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="baseScale"></param>
        /// <param name="z"></param>
        public Parralax(Vector2 worldPosition, float baseScale, float z)
        {
            z = ParralaxLerp(z);
            WorldViewPosition = GetParralaxPosition(worldPosition, z);
            ViewScale = GetParralaxScale(baseScale, z);
        }

        internal static void RefreshParralax()
        {
            GameParallax = (Main.caveParallax - 0.8f) * 5f;
        }
    }
}