using Aequus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Aequus.Common
{
    public sealed class SpriteFrameData
    {
        public Asset<Texture2D> Texture { get; private set; }
        public Rectangle Frame { get; private set; }

        public SpriteFrameData(Asset<Texture2D> texture, int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0, int sizeOffsetX = 0, int sizeOffsetY = 0)
        {
            Set(texture, horizontalFrames, verticalFrames, frameX, frameY, sizeOffsetX, sizeOffsetY);
        }

        public SpriteFrameData(Asset<Texture2D> texture, Rectangle frame)
        {
            Set(texture, frame);
        }

        public void Set(Asset<Texture2D> texture, int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0, int sizeOffsetX = 0, int sizeOffsetY = 0)
        {
            Set(texture, texture.Frame(horizontalFrames, verticalFrames, frameX, frameY, sizeOffsetX, sizeOffsetY));
        }

        public void Set(Asset<Texture2D> texture, Rectangle frame)
        {
            Texture = texture;
            Frame = frame;
        }
    }
}