using AQMod.Assets;
using AQMod.Assets.Textures;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader;

namespace AQMod.Common.Utilities
{
    [Obsolete()]
    public static class DrawUtils
    {
        public static class LegacyTextureCache
        {
            public static TEA<GlowID> Glows { get; private set; }
            public static Texture2D UI;

            internal static void Setup()
            {
                Glows = new TEA<GlowID>(GlowID.Count, "AQMod/Assets/Textures/Glows", "Glow");
                UI = ModContent.GetTexture("AQMod/Assets/Textures/UI");
            }

            internal static void Unload()
            {
                Glows = null;
                UI = null;
            }
        }

        internal static void UnloadAssets()
        {
            LegacyTextureCache.Unload();
        }
    }
}