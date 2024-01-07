using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.UI.Chat;

namespace Aequus.Core.Graphics.Commands;

public record struct DrawColorCodedStringCommand(DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false) : IDrawCommand {
    public void Draw(SpriteBatch spriteBatch) {
        ChatManager.DrawColorCodedString(spriteBatch, font, text, position, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors);
    }
}