using Microsoft.Xna.Framework;

namespace Aequus.Common.NPCs.Components;

public interface IMapHead {
    Point RenderSize => new(30, 30);
    void Draw();
}