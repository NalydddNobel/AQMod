using AQMod.Assets;
using AQMod.Common.ID;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace AQMod
{
    public static class Tex
    {
        public const string None = "AQMod/Assets/None";

        public static TextureAsset Bloom { get; private set; }
        public static TextureAsset Pixel { get; private set; }
        public static TextureAsset MeathookNPC { get; private set; }
        public static TextureAsset CrabPot { get; private set; }
        public static TextureAsset CrabPotHighlight { get; private set; }

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
            Bloom = mod.GetTextureAsset("Assets/EffectTextures/Bloom");
            Pixel = mod.GetTextureAsset("Assets/Pixel");
            MeathookNPC = mod.GetTextureAsset("Assets/UI/MeathookNPC");
            CrabPot = mod.GetTextureAsset("Assets/CrabPot");
            CrabPotHighlight = mod.GetTextureAsset("Assets/CrabPot_Highlight");
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