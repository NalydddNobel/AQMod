using Terraria.Map;

namespace Aequu2.Core.Components.NPCs;

public interface ICustomMapHead {
    void DrawMapHead(ref MapOverlayDrawContext context, ref string text);
}