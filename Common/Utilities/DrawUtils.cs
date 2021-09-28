using AQMod.Assets;
using AQMod.Assets.Enumerators;
using AQMod.Assets.Textures;
using AQMod.Content.WorldEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Common.Utilities
{
    public static class DrawUtils
    {
        public const string DistortX = "AQMod:DistortX";

        public static bool AssetsLoaded { get; private set; }
        public static List<WorldVisualEffect> WorldEffects { get; private set; }

        [Obsolete("Massive texture arrays are overrated.")]
        public static class Textures
        {
            public static TEA<ExtraID> Extras { get; private set; }
            public static TEA<GlowID> Glows { get; private set; }
            public static TEA<PlayerMaskID> PlayerMasks { get; private set; }
            public static TEA<LightID> Lights { get; private set; }
            public static TEA<TrailID> Trails { get; private set; }
            public static TEA<PlayerHeadOverlayID> PlayerHeadOverlays { get; private set; }
            public static Texture2D UI;

            internal static void Setup()
            {
                Extras = new TEA<ExtraID>(ExtraID.Count, "AQMod/Assets/Textures/Extras", "Extra");
                Glows = new TEA<GlowID>(GlowID.Count, "AQMod/Assets/Textures/Glows", "Glow");
                PlayerMasks = new TEA<PlayerMaskID>(PlayerMaskID.Count, "AQMod/Assets/Textures", "PlayerMask");
                Lights = new TEA<LightID>(LightID.Count, "AQMod/Assets/Textures/Lights", "Light");
                Trails = new TEA<TrailID>(TrailID.Count, "AQMod/Assets/Textures", "Trail");
                PlayerHeadOverlays = new TEA<PlayerHeadOverlayID>(PlayerHeadOverlayID.Count, "AQMod/Assets/Textures", "HeadOverlay");
                UI = ModContent.GetTexture("AQMod/Assets/Textures/UI");
            }

            internal static void Unload()
            {
                Extras = null;
                Glows = null;
                PlayerMasks = null;
                Lights = null;
                Trails = null;
                PlayerHeadOverlays = null;
                UI = null;
            }
        }

        internal static void FinalSetup()
        {
            WorldEffects = new List<WorldVisualEffect>();
        }

        internal static void UnloadAssets()
        {
            Textures.Unload();
            WorldEffects = null;
        }

        internal static Vector2 ScreenCenter => new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
        internal static Vector2 WorldScreenCenter => new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f);
        internal static Matrix WorldViewPoint
        {
            get
            {
                GraphicsDevice graphics = Main.graphics.GraphicsDevice;
                Vector2 zoom = Main.GameViewMatrix.Zoom;
                int width = graphics.Viewport.Width;
                int height = graphics.Viewport.Height;
                Matrix Zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
                Matrix Projection = Matrix.CreateOrthographic(width, height, 0, 1000);
                return Zoom * Projection;
            }
        }
        internal static Vector2 TileZero => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

        internal static int RandomSmokeGore(UnifiedRandom random)
        {
            return 61 + random.Next(3);
        }
    }
}