using Aequus.Content.Necromancy;
using Terraria;

namespace Aequus.Items.Accessories.Necro {
    public interface INecromancySupportAcc {
        void ApplySupportEffects(Player player, AequusPlayer aequus, NPC npc, NecromancyNPC zombie);
    }
}