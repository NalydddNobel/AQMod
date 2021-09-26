using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AQMod.Common
{
    internal delegate void DrawMethod(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth);
}