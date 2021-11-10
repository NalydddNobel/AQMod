using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Assets.DrawCode
{
    public class testdraw : IDrawType
    {
        private readonly Texture2D _texture;

        public testdraw(Texture2D texture)
        {
            _texture = texture;
        }

        public void RunDraw()
        {
            Main.spriteBatch.Draw(_texture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
