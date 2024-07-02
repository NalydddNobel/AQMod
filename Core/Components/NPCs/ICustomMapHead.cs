using Terraria.Map;

namespace Aequus.Core.Components.NPCs;

public interface ICustomMapHead {
    void DrawMapHead(ref MapOverlayDrawContext context, ref string text);
}