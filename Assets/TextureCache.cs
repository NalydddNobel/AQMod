using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Aequus.Assets
{
    public sealed class TextureCache : ILoadable
    {
        public static Asset<Texture2D>[] Bloom { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            Bloom = new Asset<Texture2D>[4];
            LoadArr(Bloom, "Aequus/Assets/Bloom", AssetRequestMode.AsyncLoad, firstUsesZero: false, doesntUse0AsTheFirstIndex: true);
        }
        private void LoadArr(Asset<Texture2D>[] arr, string path, AssetRequestMode requestMode, bool firstUsesZero = true, bool doesntUse0AsTheFirstIndex = false)
        {
            int start = 0;
            int add = 0;
            if (!firstUsesZero)
            {
                arr[0] = ModContent.Request<Texture2D>(path, requestMode);
                start++;
            }
            if (doesntUse0AsTheFirstIndex)
            {
                add++;
            }
            for (int i = start; i < arr.Length; i++)
            {
                arr[i] = ModContent.Request<Texture2D>(path + (i + add), requestMode);
            }
        }

        void ILoadable.Unload()
        {
            Bloom = null;
        }
    }
}