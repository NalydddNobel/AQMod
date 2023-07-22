using Aequus.NPCs.BossMonsters.Crabson.Common;
using Terraria;

namespace Aequus;

public partial class AequusSystem {
    public static int CrabsonNPC = -1;

    public void PreUpdateEntities_CheckCrabson() {
        if (CrabsonNPC == -1 || (Main.npc[CrabsonNPC].active && Main.npc[CrabsonNPC].ModNPC is ICrabson)) {
            return;
        }

        CrabsonNPC = -1;
    }
}