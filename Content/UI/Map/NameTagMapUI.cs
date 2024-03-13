using Aequus.Common.Renaming;
using Microsoft.Xna.Framework;
using Terraria.Map;
using Terraria.UI;

namespace Aequus.Content.UI.Map;

public class NameTagMapUI : ModMapLayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return true;
    }

    public override void Draw(ref MapOverlayDrawContext context, ref string text) {
        foreach (var npc in RenamingSystem.RenamedNPCs) {
            if (context.Draw(AequusTextures.NameTagBlip, new Vector2(npc.Value.tileX + 0.5f, npc.Value.tileY + 0.5f), Alignment.Center).IsMouseOver) {
                text = $"{npc.Value.customName} ({LanguageDatabase.GetNPCNameValue(npc.Value.type)})";
            }
        }
    }
}