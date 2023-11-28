using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Graphics;

namespace Aequus.Core.Graphics.Commands;

public sealed class DrawCommandHandler {
    private readonly Queue<IDrawCommand> _commands = new();

    public int Count => _commands.Count;

    public void Draw(DrawData drawData) {
        _commands.Enqueue(new DrawDataCommand(drawData));
    }

    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float inactiveLayerDepth = 0f) {
        Draw(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth));
    }

    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float inactiveLayerDepth = 0f) {
        Draw(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth));
    }

    public void DrawColorCodedString(DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false) {
        _commands.Enqueue(new DrawColorCodedStringCommand(font, text, position, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors));
    }

    public void DrawBasicVertexLine(Texture2D texture, Vector2[] lineSegments, float[] lineRotations, VertexStrip.StripColorFunction getColor, VertexStrip.StripHalfWidthFunction getWidth, Vector2 offset = default, bool includeBacksides = true, bool tryStoppingOddBug = true) {
        _commands.Enqueue(new DrawBasicVertexStripCommand(texture, lineSegments, lineRotations, getColor, getWidth, offset, includeBacksides, tryStoppingOddBug));
    }

    public IDrawCommand Dequeue() {
        return _commands.Dequeue();
    }

    public bool TryDequeue(out IDrawCommand command) {
        return _commands.TryDequeue(out command);
    }

    public IDrawCommand Invoke(SpriteBatch spriteBatch) {
        lock (_commands) {
            var value = Dequeue();
            value.Draw(spriteBatch);
            return value;
        }
    }

    public bool TryInvoke(SpriteBatch spriteBatch) {
        lock (_commands) {
            if (TryDequeue(out var command)) {
                command.Draw(spriteBatch);
                return true;
            }
            return false;
        }
    }

    public bool TryInvokeOut(SpriteBatch spriteBatch, out IDrawCommand command) {
        lock (_commands) {
            if (TryDequeue(out command)) {
                command.Draw(spriteBatch);
                return true;
            }
            return false;
        }
    }

    public void InvokeAll(SpriteBatch spriteBatch) {
        lock (_commands) {
            while (TryInvoke(spriteBatch)) ;
        }
    }

    public void Clear() {
        lock (_commands) {
            _commands.Clear();
        }
    }
}