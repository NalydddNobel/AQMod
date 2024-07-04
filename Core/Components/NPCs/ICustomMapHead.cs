using Terraria.Map;

namespace AequusRemake.Core.Components.NPCs;

public interface ICustomMapHead {
    void DrawMapHead(ref MapOverlayDrawContext context, ref string text);
}