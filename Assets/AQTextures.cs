using AQMod.Common.Graphics.Particles;
using AQMod.Effects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public static class AQTextures
    {
        public const string None = "Assets/empty";
        public const string Error = "Assets/error";

        public static Dictionary<ParticleTex, Texture2D> Particles { get; private set; }
        public static Dictionary<LightTex, Texture2D> Lights { get; private set; }
        public static Dictionary<TrailTex, Texture2D> Trails { get; private set; }

        public static Texture2D Pixel { get; private set; }

        internal static void Load()
        {
            Pixel = ModContent.GetTexture("AQMod/Assets/Pixel");
            LoadDictionaries();
        }

        private static void LoadDictionaries()
        {
            Particles = FillDictionary(ParticleTex.Count, "AQMod/Assets/Particles/Particle_");
            Lights = FillDictionary(LightTex.Count, "AQMod/Assets/Lights/Light_");
            Trails = FillDictionary(TrailTex.Count, "AQMod/Assets/Trails/Trail_");
        }

        private static Dictionary<TEnum, Texture2D> FillDictionary<TEnum>(TEnum count, string pathWithoutNumbers) where TEnum : Enum
        {
            try
            {
                int max = count.GetHashCode();
                var d = new Dictionary<TEnum, Texture2D>(max);
                for (ushort i = 0; i < max; i++)
                {
                    if (AQMod.Unloading)
                    {
                        return null;
                    }
                    d.Add(i.ToEnum<TEnum>(), ModContent.GetTexture(pathWithoutNumbers + i));
                }
                return d;
            }
            catch
            {
                return null;
            }
        }

        internal static void Unload()
        {
            Pixel = null;
            Particles = null;
            Lights = null;
            Trails = null;
        }
    }
}