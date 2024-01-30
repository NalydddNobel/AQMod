using Terraria.Map;

namespace Aequus.Common.NPCs.Components;

public interface ICustomMapHead {
    void DrawMapHead(ref MapOverlayDrawContext context, ref string text);
}