using AQMod.Assets;
using System.Reflection;

namespace AQMod
{
    public static class Tex
    {
        public const string None = "AQMod/Assets/None";

        public static TextureAsset Pixel { get; private set; }
        public static TextureAsset MeathookNPC { get; private set; }
        public static TextureAsset CrabPot { get; private set; }
        public static TextureAsset CrabPotHighlight { get; private set; }

        internal static void Load(AQMod mod)
        {
            Pixel = mod.GetTextureAsset("Assets/Pixel");
            MeathookNPC = mod.GetTextureAsset("Assets/UI/MeathookNPC");
            CrabPot = mod.GetTextureAsset("Assets/CrabPot");
            CrabPotHighlight = mod.GetTextureAsset("Assets/CrabPot_Highlight");
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