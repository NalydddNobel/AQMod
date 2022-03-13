using AQMod.Assets;
using AQMod.Common.ID;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace AQMod
{
    public static class Tex
    {
        public const string None = "AQMod/Assets/None";

        public class Lights
        {
            public static Texture2D Spotlight30x30 => _lights[0];
            public static Texture2D Spotlight66x66 => _lights[1];
            public static Texture2D Spotlight66x66Negative => _lights[2];
            public static Texture2D Spotlight240x66 => _lights[3];
            public static Texture2D Spotlight12x66 => _lights[4];
            public static Texture2D Spotlight33x24 => _lights[5];
            public static Texture2D LightLine => _lights[6];
            public static Texture2D Spotlight15x15 => _lights[7];
            public static Texture2D Spotlight20x20 => _lights[8];
            public static Texture2D Spotlight24x24 => _lights[9];
            public static Texture2D Spotlight36x36 => _lights[10];
            public static Texture2D Spotlight48x48 => _lights[11];
            public static Texture2D Spotlight100x100 => _lights[12];
            public static Texture2D Spotlight250x250 => _lights[13];
            public static Texture2D Spotlight80x80 => _lights[14];
            public static Texture2D Spotlight8x8 => _lights[15];
            public const int LightPillarSmall = 16;
            public const int LightPillar = 17;
            public const int Spotlight10x100 = 18;
            public const int Spotlight10x50 = 19;
            public const int Spotlight20x100 = 20;
            public const int Spotlight20x50 = 21;
            public const int Spotlight25x50 = 22;
            public const int Spotlight33x50 = 23;
            public const int Spotlight40x100 = 24;
            public const int Spotlight50x100 = 25;
            public const int Spotlight66x100 = 26;
            public const int Spotlight80x80Half = 27;
            public const int Spotlight80x80HalfCropped = 28;
            public const int Ray40 = 29;
            public const int Ray40Half = 30;
            public const int Ray80 = 31;
            public const int Ray80Half = 32;
            public const int Ray80WideBottom = 33;
            public const int Ray80WideBottomHalf = 34;
            public const int Count = 35;
        }

        public static TextureAsset Pixel { get; private set; }
        public static TextureAsset MeathookNPC { get; private set; }
        public static TextureAsset CrabPot { get; private set; }
        public static TextureAsset CrabPotHighlight { get; private set; }
        private static Texture2D[] _lights;

        private static Texture2D[] FillArray<T>(string pathWithoutNumbers) where T : class
        {
            int count = SetConstantsIdentityAttribute.GetCount<T>();
            var t = new Texture2D[count];
            for (int i = 0; i < count; i++)
            {
                t[i] = AQMod.LoggableTexture("AQMod/Assets/" + pathWithoutNumbers + "_" + i);
            }
            return t;
        }
        internal static void Load(AQMod mod)
        {
            Pixel = mod.GetTextureAsset("Assets/Pixel");
            MeathookNPC = mod.GetTextureAsset("Assets/UI/MeathookNPC");
            CrabPot = mod.GetTextureAsset("Assets/CrabPot");
            CrabPotHighlight = mod.GetTextureAsset("Assets/CrabPot_Highlight");
            _lights = FillArray<LightTex>("Lights/Light");
        }

        private static void Discard(ref Texture2D[] value)
        {
            if (value == null)
            {
                return;
            }
            for (int i = 0; i < value.Length; i++)
            {
                Discard(ref value[i]);
            }
            value = null;
        }
        private static void Discard(ref Texture2D value)
        {
            value?.Dispose();
            value = null;
        }
        internal static void Unload()
        {
            Discard(ref _lights);
            foreach (var p in typeof(Tex).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                var tA = (TextureAsset)p.GetValue(null);
                if (tA != null)
                {
                    tA.Dispose();
                    p.SetValue(null, null);
                }
            }
        }
    }
}