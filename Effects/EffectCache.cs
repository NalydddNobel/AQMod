using AQMod.Assets.Textures;
using AQMod.Common.Config;
using AQMod.Common.Utilities;
using AQMod.Items.Vanities.Dyes;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AQMod.Effects
{
    public class EffectCache
    {
        public static EffectCache Instance;

        public Effect Trailshader { get; private set; }
        public Effect Scroll { get; private set; }
        public Effect Hypno { get; private set; }
        public Effect Outline { get; private set; }
        public Effect ScreenDistort { get; private set; }
        public Effect Portal { get; private set; }
        public Effect ColorDistort { get; private set; }
        public Effect Spotlight { get; private set; }

        public EffectCache()
        {
        }

        public EffectCache(AQMod mod, AQConfigClient client, ILog logger)
        {
            Setup(mod, client, logger);
            BindDyes(mod, client, logger);
            AddFilters(mod, client, logger);
        }

        public void Setup(AQMod mod, AQConfigClient client, ILog logger)
        {
            logger.Info("Loading Shaders...");
            if (client.SpotlightShader)
            {
                try
                {
                    Spotlight = mod.GetEffect("Effects/Spotlight");
                }
                catch (Exception e)
                {
                    throw new Exception("Couldn't load Spotlight Shader, try disabling it in the Client Config.", e);
                }
            }
            if (client.ColorDistortShader)
            {
                try
                {
                    ColorDistort = mod.GetEffect("Effects/ColorDistort");
                }
                catch (Exception e)
                {
                    throw new Exception("Couldn't load Color Distort Shader, try disabling it in the Client Config.", e);
                }
            }
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
            if (client.PortalShader)
            {
                try
                {
                    Portal = mod.GetEffect("Effects/Portal");
                }
                catch (Exception e)
                {
                    throw new Exception("Couldn't load Portal Shader, try disabling it in the Client Config.", e);
                }
            }
            if (client.ScrollShader)
            {
                try
                {
                    Scroll = mod.GetEffect("Effects/Scroll");
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
                }
                catch (Exception e)
                {
                    throw new Exception("Couldn't load Screen Distort Shader, try disabling it in the Client Config.", e);
                }
            }
        }

        public void BindDyes(AQMod mod, AQConfigClient client, ILog logger)
        {
            logger.Info("Binding Shaders to Dyes...");
            if (client.ColorDistortShader)
            {
                GameShaders.Armor.BindShader(ModContent.ItemType<DiscoDye>(), new ArmorShaderData(new Ref<Effect>(ColorDistort), "RainbowPass").UseOpacity(1f));
                GameShaders.Armor.BindShader(ModContent.ItemType<BreakdownDye>(), new ArmorShaderData(new Ref<Effect>(ColorDistort), "ColorDistortPass").UseOpacity(1f));
            }
            if (client.ScrollShader)
            {
                GameShaders.Armor.BindShader(ModContent.ItemType<ScrollDye>(), new ArmorShaderData(new Ref<Effect>(Scroll), "ScrollPass"));
                GameShaders.Armor.BindShader(ModContent.ItemType<EnchantedDye>(), new ArmorShaderData(new Ref<Effect>(Scroll), "ImageScrollPass").UseTexture2D(DrawUtils.Textures.Extras[ExtraID.EnchantedGlimmer]));
                GameShaders.Armor.BindShader(ModContent.ItemType<HellBeamDye>(), new LightSourceAsThirdColorVariableArmorShaderData(new Ref<Effect>(Scroll), "ShieldBeamsPass", new Vector3(0.3f, 0.2f, 0f))
                    .UseColor(new Vector3(1f, 0.8f, 0.1f)).UseSecondaryColor(1.8f, 0.8f, 0.6f));
            }
            if (client.HypnoShader)
            {
                GameShaders.Armor.BindShader(ModContent.ItemType<HypnoDye>(), new ArmorShaderData(new Ref<Effect>(Hypno), "HypnoPass"));
                GameShaders.Armor.BindShader(ModContent.ItemType<SimplifiedDye>(), new ArmorShaderData(new Ref<Effect>(Hypno), "SimplifyPass"));
            }
            if (client.OutlineShader)
            {
                GameShaders.Armor.BindShader(ModContent.ItemType<OutlineDye>(), new ArmorShaderData(new Ref<Effect>(Outline), "OutlinePass"));
                GameShaders.Armor.BindShader(ModContent.ItemType<RainbowOutlineDye>(), new DynamicColorArmorShaderData(new Ref<Effect>(Outline), "OutlineColorPass", () => Main.DiscoColor.ToVector3()));
            }
        }

        public const string DistortXFilter = "AQMod:DistortX";

        public void AddFilters(AQMod mod, AQConfigClient client, ILog logger)
        {
            logger.Info("Loading filters...");
            if (client.ScreenDistortShader)
                Filters.Scene[DistortXFilter] = new Filter(new ScreenShaderData(new Ref<Effect>(ScreenDistort), "DistortXPass").UseIntensity(1f), EffectPriority.Low);
        }
    }
}