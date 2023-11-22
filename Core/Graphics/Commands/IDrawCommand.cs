using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Core.Graphics.Commands; 

public interface IDrawCommand {
    void Draw(SpriteBatch spriteBatch);
}