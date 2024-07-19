using Aequus.Common.UI;
using Aequus.Common.UI;
using Terraria.UI;

namespace Aequus.Content.Necromancy;
public class NecromancyInterface : NewUILayer {
    protected override bool DrawSelf() {
        bool anyDrawn = false;
        for (int i = Main.maxNPCs - 1; i >= 0; i--) {
            if (Main.npc[i].active && (Main.npc[i].realLife == -1 || Main.npc[i].realLife == i) && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var n) && n.isZombie && !n.statFreezeLifespan) {
                n.DrawHealthbar(Main.npc[i], Main.spriteBatch, Main.screenPosition);
                anyDrawn = true;
            }
        }

        if (!anyDrawn) {
            Deactivate();
        }

        return true;
    }

    public NecromancyInterface() : base("Necromancy", AequusUI.InterfaceLayers.EntityHealthBars_16, InterfaceScaleType.Game) { }
}
