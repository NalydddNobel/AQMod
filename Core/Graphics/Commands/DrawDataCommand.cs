using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Aequus.Core.Graphics.Commands; 

public record struct DrawDataCommand(DrawData drawData) : IDrawCommand {
    public void Draw(SpriteBatch spriteBatch) {
        drawData.Draw(spriteBatch);
    }
}