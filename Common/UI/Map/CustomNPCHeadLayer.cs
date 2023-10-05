using Aequus.Common.NPCs.Components;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;

namespace Aequus.Common.UI.Map;

public class CustomNPCHeadLayer : ModMapLayer {
    public override void Draw(ref MapOverlayDrawContext context, ref string text) {
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].ModNPC is ICustomMapHead mapHead) {
                mapHead.DrawMapHead(ref context, ref text);
            }
        }
    }
}