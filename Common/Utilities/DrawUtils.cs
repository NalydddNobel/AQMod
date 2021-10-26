using AQMod.Assets;
using AQMod.Assets.Textures;
using System;

namespace AQMod.Common.Utilities
{
    [Obsolete("Remove this.")]
    public static class DrawUtils
    {
        public static class LegacyTextureCache
        {
            public static TEA<GlowID> Glows { get; private set; }

            internal static void Setup()
            {
                Glows = new TEA<GlowID>(GlowID.Count, "AQMod/Assets/Textures/Glows", "Glow");
            }

            internal static void Unload()
            {
                Glows = null;
            }
        }

        internal static void UnloadAssets()
        {
            LegacyTextureCache.Unload();
        }
    }
}