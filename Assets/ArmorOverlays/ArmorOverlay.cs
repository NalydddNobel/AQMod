using Terraria.ModLoader;

namespace AQMod.Assets.ArmorOverlays
{
    public abstract class ArmorOverlay
    {
        public readonly TextureAsset Texture;

        public ArmorOverlay(string path)
        {
            Texture = new TextureAsset(path);
        }

        public ArmorOverlay(TextureAsset texture)
        {
            Texture = texture;
        }

        public abstract void Draw(PlayerDrawInfo info);
    }
}