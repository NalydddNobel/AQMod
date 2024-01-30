using Aequus.Common.NPCs.Components;
using Terraria.Map;

namespace Aequus.Core.UI.Map;

public class CustomNPCHeadLayer : ModMapLayer {
    public override void Draw(ref MapOverlayDrawContext context, ref System.String text) {
        for (System.Int32 i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].ModNPC is ICustomMapHead mapHead) {
                mapHead.DrawMapHead(ref context, ref text);
            }
        }
    }
}