using Terraria.Map;

namespace AequusRemake.Systems.NPCs;

public interface ICustomMapHead {
    void DrawMapHead(ref MapOverlayDrawContext context, ref string text);
}