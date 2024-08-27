using Aequus.Items.Equipment.Accessories.Money.FoolsGoldRing;

namespace Aequus;

public partial class AequusNPC {
    public void ProcFoolsGoldRing(NPC npc, AequusNPC aequus, Player player, AequusPlayer aequusPlayer) {
        if (npc.value <= 0f || !player.HasBuff<FoolsGoldRingBuff>()) {
            return;
        }

        aequusPlayer.accFoolsGoldRingCharge = 0;

        npc.value *= 3f;
        aequus.dropRerolls += 3f;
        doLuckyDropsEffect = true;

        if (Main.netMode != NetmodeID.Server) {
            FoolsGoldRing.OnKillEffects((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
        }
        else {
            ModContent.GetInstance<FoolsGoldRingEffectPacket>().Send(npc);
        }

        doLuckyDropsEffect = false;
    }
}