using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets.PlayerLayers.EquipOverlays
{
    public abstract class EquipOverlay
    {
        public readonly Ref<Texture2D> Asset;

        public EquipOverlay(string path)
        {
            Asset = new Ref<Texture2D>(ModContent.GetTexture(path));
        }

        public EquipOverlay(Texture2D texture)
        {
            Asset = new Ref<Texture2D>(texture);
        }

        public EquipOverlay(Ref<Texture2D> asset)
        {
            Asset = asset;
        }

        public abstract void Draw(PlayerDrawInfo info);
    }
}