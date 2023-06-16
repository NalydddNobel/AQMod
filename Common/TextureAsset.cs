using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common {
    public class TextureAsset {
        private Asset<Texture2D> texture;
        public readonly string Path;

        public Asset<Texture2D> Asset => (texture ??= ModContent.Request<Texture2D>(Path, AssetRequestMode.ImmediateLoad));
        public Texture2D Value => Asset.Value;

        public int Width => Value.Width;
        public int Height => Value.Height;
        public Rectangle Bounds => Value.Bounds;

        public TextureAsset(string path) {
            Path = path;
        }

        public Vector2 Size() {
            return Value.Size();
        }

        public Rectangle Frame(int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0, int sizeOffsetX = 0, int sizeOffsetY = 0) {
            var val = Value;
            int num = val.Width / horizontalFrames;
            int num2 = val.Height / verticalFrames;
            return new Rectangle(num * frameX, num2 * frameY, num + sizeOffsetX, num2 + sizeOffsetY);
        }

        internal void Unload() {
            texture = null;
        }

        public static implicit operator Asset<Texture2D>(TextureAsset value) {
            return value.Asset;
        }
        public static implicit operator Texture2D(TextureAsset value) {
            return value.Value;
        }
    }
}