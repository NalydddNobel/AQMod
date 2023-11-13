using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Core;

public delegate void DrawDelegate(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float inactiveLayerDepth = 0f);