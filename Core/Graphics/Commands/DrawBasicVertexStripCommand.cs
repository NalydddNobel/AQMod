using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;

namespace Aequus.Core.Graphics.Commands;

public record struct DrawBasicVertexStripCommand(Texture2D texture, Vector2[] lineSegments, float[] lineRotations, VertexStrip.StripColorFunction getColor, VertexStrip.StripHalfWidthFunction getWidth, Vector2 offset = default, bool includeBacksides = true, bool tryStoppingOddBug = true) : IDrawCommand {
    public void Draw(SpriteBatch spriteBatch) {
        DrawHelper.DrawBasicVertexLine(texture, lineSegments, lineRotations, getColor, getWidth, offset, includeBacksides, tryStoppingOddBug);
    }
}