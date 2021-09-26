using AQMod.Assets.Enumerators;
using AQMod.Assets.Textures;
using AQMod.Common.Config;
using AQMod.Common.WorldEffects;
using AQMod.Effects;
using AQMod.Items.GrapplingHooks;
using AQMod.Items.Vanities.Dyes;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public static class SpriteUtils
    {
        public const string DistortX = "AQMod:DistortX";

        public static bool AssetsLoaded { get; private set; }
        public static List<WorldVisualEffect> WorldEffects { get; private set; }

        public static class Textures
        {
            public static TEA<ExtraID> Extras { get; private set; }
            public static TEA<GlowID> Glows { get; private set; }
            public static TEA<PlayerMaskID> PlayerMasks { get; private set; }
            public static TEA<LightID> Lights { get; private set; }
            public static TEA<TrailID> Trails { get; private set; }
            public static TEA<PlayerHeadOverlayID> PlayerHeadOverlays { get; private set; }
            public static Texture2D Pixel;
            public static Texture2D UI;

            internal static void Setup()
            {
                Extras = new TEA<ExtraID>(ExtraID.Count, "AQMod/Assets/Textures/Extras", "Extra");
                Glows = new TEA<GlowID>(GlowID.Count, "AQMod/Assets/Textures/Glows", "Glow");
                PlayerMasks = new TEA<PlayerMaskID>(PlayerMaskID.Count, "AQMod/Assets/Textures", "PlayerMask");
                Lights = new TEA<LightID>(LightID.Count, "AQMod/Assets/Textures/Lights", "Light");
                Trails = new TEA<TrailID>(TrailID.Count, "AQMod/Assets/Textures", "Trail");
                PlayerHeadOverlays = new TEA<PlayerHeadOverlayID>(PlayerHeadOverlayID.Count, "AQMod/Assets/Textures", "HeadOverlay");
                Pixel = ModContent.GetTexture("AQMod/Assets/Textures/Pixel");
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
                Pixel = null;
                UI = null;
            }
        }

        public static class Effects
        {
            public static Effect Trailshader { get; private set; }
            public static Effect Scroll { get; private set; }
            public static Effect Hypno { get; private set; }
            public static Effect Outline { get; private set; }
            public static Effect ScreenDistort { get; private set; }

            internal static void LoadShaders(AQMod mod, AQConfigClient client, ILog logger)
            {
                logger.Info("Binding Shaders to Dyes...");
                if (client.TrailShader)
                {
                    try
                    {
                        Trailshader = mod.GetEffect("Effects/Trailshader");
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Couldn't load Trail Shader, try disabling it in the Client Config.", e);
                    }
                }
                if (client.ScrollShader)
                {
                    try
                    {
                        Scroll = mod.GetEffect("Effects/Scroll");
                        GameShaders.Armor.BindShader(ModContent.ItemType<ScrollDye>(), new ArmorShaderData(new Ref<Effect>(Scroll), "ScrollPass"));
                        GameShaders.Armor.BindShader(ModContent.ItemType<EnchantedDye>(), new ArmorShaderData(new Ref<Effect>(Scroll), "ImageScrollPass").UseTexture2D(Textures.Extras[ExtraID.EnchantedGlimmer]));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Couldn't load Scroll Shader, try disabling it in the Client Config.", e);
                    }
                }
                if (client.HypnoShader)
                {
                    try
                    {
                        Hypno = mod.GetEffect("Effects/Hypno");
                        GameShaders.Armor.BindShader(ModContent.ItemType<HypnoDye>(), new ArmorShaderData(new Ref<Effect>(Hypno), "HypnoPass"));
                        GameShaders.Armor.BindShader(ModContent.ItemType<SimplifiedDye>(), new ArmorShaderData(new Ref<Effect>(Hypno), "SimplifyPass"));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Couldn't load Hypno Shader, try disabling it in the Client Config.", e);
                    }
                }
                if (client.OutlineShader)
                {
                    try
                    {
                        Outline = mod.GetEffect("Effects/Outline");
                        GameShaders.Armor.BindShader(ModContent.ItemType<OutlineDye>(), new ArmorShaderData(new Ref<Effect>(Outline), "OutlinePass"));
                        GameShaders.Armor.BindShader(ModContent.ItemType<RainbowOutlineDye>(), new DynamicColorArmorShaderData(new Ref<Effect>(Outline), "OutlineColorPass", () => Main.DiscoColor.ToVector3()));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Couldn't load Outline Shader, try disabling it in the Client Config.", e);
                    }
                }
                if (client.ScreenDistortShader)
                {
                    try
                    {
                        ScreenDistort = mod.GetEffect("Effects/ScreenDistort");
                        Filters.Scene[DistortX] = new Filter(new ScreenShaderData(new Ref<Effect>(ScreenDistort), "DistortXPass").UseIntensity(1f), EffectPriority.Low);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Couldn't load Screen Distort Shader, try disabling it in the Client Config.", e);
                    }
                }
            }

            internal static void UnloadShaders()
            {
                Trailshader = null;
                Scroll = null;
                Hypno = null;
                Outline = null;
                ScreenDistort = null;
            }
        }

        internal static void LoadAssets(AQMod mod, AQConfigClient client, ILog logger)
        {
            WorldEffects = new List<WorldVisualEffect>();
            Textures.Setup();
            ChainTextures.Setup();
            Effects.LoadShaders(mod, client, logger);
            AssetsLoaded = true;
        }

        internal static void UnloadAssets()
        {
            AssetsLoaded = false;
            Textures.Unload();
            Effects.UnloadShaders();
            WorldEffects = null;
        }

        internal static Vector2 ScreenCenter => new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
        internal static Vector2 WorldScreenCenter => new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f);
        public static Matrix WorldViewPoint
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

        public static Texture2D GetTexture(this ModItem ModItem)
        {
            return ModItem.item.GetTexture();
        }

        public static Texture2D GetTexture(this Item Item)
        {
            return Main.itemTexture[Item.type];
        }

        public static Texture2D GetTexture(this ModProjectile ModProjectile)
        {
            return ModProjectile.projectile.GetTexture();
        }

        public static Texture2D GetTexture(this Projectile Projectile)
        {
            return Main.projectileTexture[Projectile.type];
        }
    }
}