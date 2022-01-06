using AQMod.Common.Utilities;
using AQMod.Effects;
using AQMod.Effects.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public static class AQTextures
    {
        public const string None = "Assets/empty";
        public const string Error = "Assets/error";

        public static Texture2D[] Particles { get; private set; }
        public static Texture2D[] Lights { get; private set; }
        public static Texture2D[] Trails { get; private set; }

        public static Texture2D Pixel { get; private set; }

        internal static void Load()
        {
            Pixel = ModContent.GetTexture("AQMod/Assets/Pixel");
            LoadDictionaries();
        }

        private static void LoadDictionaries()
        {
            Particles = FillArray(ParticleTex.Count, "Particles/Particle_");
            Lights = FillArray(LightTex.Count, "Lights/Light_");
            Trails = FillArray(TrailTex.Count, "Trails/Trail_");
        }

        internal static Texture2D[] FillArray(int count, string pathWithoutNumbers)
        {
            var t = new Texture2D[count];
            for (int i = 0; i < count; i++)
            {
                t[i] = ModContent.GetTexture("AQMod/Assets/" + pathWithoutNumbers + i);
            }
            return t;
        }

        internal static Dictionary<TEnum, Texture2D> FillDictionary<TEnum>(TEnum count, string pathWithoutNumbers) where TEnum : Enum
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