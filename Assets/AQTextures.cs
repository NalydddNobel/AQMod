using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public sealed class AQTextures : ILoadable
    {
        public const string None = "Assets/empty";
        public const string Error = "Assets/error";

        public static AssetDictionary<LightTex, Texture2D> Lights { get; private set; }

        public static Asset<Texture2D> Pixel { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            Pixel = ModContent.Request<Texture2D>("AQMod/Assets/Pixel");
            Lights = new AssetDictionary<LightTex, Texture2D>("AQMod/Assets/Lights/Light_");
        }

        void ILoadable.Unload()
        {
            Pixel = null;
            Lights = null;
        }
    }
}