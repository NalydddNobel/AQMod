using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using Terraria;

namespace Aequus.Common {
    public class TextureAsset : TemplateAsset<Texture2D> {
        public int Width => Value.Width;
        public int Height => Value.Height;
        public Rectangle Bounds => Value.Bounds;

        public TextureAsset(string path) : base(path) {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 GetCenteredFrameOrigin(int horizontalFrames = 1, int verticalFrames = 1) {
            var size = Size();
            return new Vector2(size.X / horizontalFrames / 2f, size.Y / verticalFrames / 2f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 Size() {
            return Value.Size();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle Frame(int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0, int sizeOffsetX = 0, int sizeOffsetY = 0) {
            var val = Value;
            int num = val.Width / horizontalFrames;
            int num2 = val.Height / verticalFrames;
            return new Rectangle(num * frameX, num2 * frameY, num + sizeOffsetX, num2 + sizeOffsetY);
        }
    }
}