using Terraria.Map;
using Terraria.UI;

namespace Aequus.Systems.Renaming;

public class NameTagMapUI : ModMapLayer {
    public override void Draw(ref MapOverlayDrawContext context, ref string text) {
        foreach (var npc in RenamingSystem.RenamedNPCs) {
            if (context.Draw(AequusTextures.NameTagBlip, new Vector2(npc.Value.tileX + 0.5f, npc.Value.tileY + 0.5f), Alignment.Center).IsMouseOver) {
                text = $"{npc.Value.customName} ({Lang.GetNPCNameValue(npc.Value.type)})";
            }
        }
    }
}