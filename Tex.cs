using AQMod.Assets;
using System.Reflection;

namespace AQMod
{
    public static class Tex
    {
        public const string None = "AQMod/Assets/None";

        public static TextureAsset Pixel { get; private set; }
        public static TextureAsset MeathookNPC { get; private set; }

        internal static void InternalSetup(AQMod mod)
        {
            Pixel = mod.GetTextureAsset("Assets/Pixel");
            MeathookNPC = mod.GetTextureAsset("Assets/UI/MeathookNPC");
        }

        internal static void Unload()
        {
            foreach (var p in typeof(Tex).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                var tA = (TextureAsset)p.GetValue(null);
                tA.Dispose();
                p.SetValue(null, null);
            }
        }
    }
}