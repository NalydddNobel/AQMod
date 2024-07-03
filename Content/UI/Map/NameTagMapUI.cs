using Aequu2.Content.Systems.Renaming;
using Terraria.Map;
using Terraria.UI;

namespace Aequu2.Content.UI.Map;

public class NameTagMapUI : ModMapLayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return true;
    }

    public override void Draw(ref MapOverlayDrawContext context, ref string text) {
        foreach (var npc in RenamingSystem.RenamedNPCs) {
            if (context.Draw(AequusTextures.NameTagBlip, new Vector2(npc.Value.tileX + 0.5f, npc.Value.tileY + 0.5f), Alignment.Center).IsMouseOver) {
                text = $"{npc.Value.customName} ({Lang.GetNPCNameValue(npc.Value.type)})";
            }
        }
    }
}