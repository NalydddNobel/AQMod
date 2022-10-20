using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Common
{
    public class TextureColorData
    {
        private Color[] data;
        private Texture2D texture;
        private Rectangle boundsCache;

        public TextureColorData(Texture2D texture)
        {
            data = new Color[texture.Width * texture.Height];
            this.texture = texture;
            boundsCache = texture.Bounds;
        }

        public Color this[int x, int y]
        {
            get => data[y * texture.Width + x];
            set => data[y * texture.Width + x] = value;
        }

        public bool CheckTexture(Texture2D texture2)
        {
            return data.Length == texture2.Width * texture2.Height;
        }

        public bool InBounds(int x, int y)
        {
            return boundsCache.Contains(x, y);
        }

        public void ApplyChanges()
        {
            texture.SetData(data);
        }

        public void RefreshTexture(Texture2D texture)
        {
            texture.GetData(data);
        }
    }
}
