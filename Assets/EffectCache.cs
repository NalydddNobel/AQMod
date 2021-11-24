using AQMod.Common.Config;
using log4net;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace AQMod.Assets
{
    public static class EffectCache
    {
        public static Effect ParentPixelShader { get; internal set; }

        public static Effect Trailshader { get; private set; }
        public static Effect Scroll { get; private set; }
        public static Effect Hypno { get; private set; }
        public static Effect Outline { get; private set; }
        public static Effect ScreenDistort { get; private set; }
        public static Effect Portal { get; private set; }
        public static Effect ColorDistort { get; private set; }
        public static Effect Spotlight { get; private set; }

        internal static void Setup(AQMod aQMod)
        {
            ParentPixelShader = aQMod.GetEffect("Effects/Dyes/ParentDyeShader");
        }

        internal static void Setup(AQMod mod, AQConfigClient client, ILog logger)
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

        public const string DistortXFilter = "AQMod:DistortX";

        public static void AddFilters(AQMod mod, AQConfigClient client, ILog logger)
        {
            logger.Info("Loading filters...");
            if (client.ScreenDistortShader)
                Filters.Scene[DistortXFilter] = new Filter(new ScreenShaderData(new Ref<Effect>(ScreenDistort), "DistortXPass").UseIntensity(1f), EffectPriority.Low);
        }
    }
}