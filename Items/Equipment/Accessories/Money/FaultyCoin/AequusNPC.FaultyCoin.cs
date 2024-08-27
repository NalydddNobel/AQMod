using Aequus.Items.Equipment.Accessories.Money.FaultyCoin;

namespace Aequus;

public partial class AequusNPC {
    public void ProcFaultyCoin(NPC npc, AequusNPC aequus, Player player, AequusPlayer aequusPlayer) {
        if (npc.value <= 0f || !player.HasBuff<FaultyCoinBuff>()) {
            return;
        }

        npc.value *= 2f;
    }
}