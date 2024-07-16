using Terraria.Map;

namespace Aequus.NPCs;

public interface ICustomMapHead {
    void DrawMapHead(ref MapOverlayDrawContext context, ref string text);
}