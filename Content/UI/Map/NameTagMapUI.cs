using Aequus.Common.Renaming;
using Terraria.Map;
using Terraria.UI;

namespace Aequus.Content.UI.Map;

public class NameTagMapUI : ModMapLayer {
    public override System.Boolean IsLoadingEnabled(Mod mod) {
        return true;
    }

    public override void Draw(ref MapOverlayDrawContext context, ref System.String text) {
        foreach (var npc in RenamingSystem.RenamedNPCs) {
            if (context.Draw(AequusTextures.NameTagBlip, new Vector2(npc.Value.tileX + 0.5f, npc.Value.tileY + 0.5f), Alignment.Center).IsMouseOver) {
                text = $"{npc.Value.customName} ({Lang.GetNPCNameValue(npc.Value.type)})";
            }
        }
    }
}