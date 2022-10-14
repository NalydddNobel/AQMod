using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Aequus.Graphics
{
    public class TextureInfo
    {
        public Asset<Texture2D> Texture;
        public Rectangle Frame;
        public Vector2 Origin;

        public int FramesX => Texture.Value.Width / Frame.Width;
        public int FramesY => Texture.Value.Height / Frame.Height;
        public Rectangle SnippedFrame => new Rectangle(Frame.X, Frame.Y, Frame.Width - 2, Frame.Y - 2);

        public TextureInfo(Asset<Texture2D> texture, Rectangle frame, Vector2 origin)
        {
            Texture = texture;
            Frame = frame;
            Origin = origin;
        }

        public TextureInfo(Asset<Texture2D> texture) : this(texture, texture.Value.Bounds, Vector2.Zero)
        {
        }

        public TextureInfo(Asset<Texture2D> texture, int horizontalFrames, int verticalFrames, Vector2 origin) : this(texture, texture.Value.Frame(horizontalFrames, verticalFrames), origin)
        {
        }

        public TextureInfo(Asset<Texture2D> texture, Rectangle frame, float originFracX, float originFracY) : this(texture, frame, new Vector2(originFracX * frame.X, originFracY * frame.Y))
        {
        }

        public TextureInfo(Asset<Texture2D> texture, int horizontalFrames, int verticalFrames, float originFracX, float originFracY)
        {
            Texture = texture;
            Frame = texture.Value.Frame(horizontalFrames, verticalFrames);
            Origin = new Vector2(originFracX * Frame.Width, originFracY * Frame.Height);
        }
    }
}