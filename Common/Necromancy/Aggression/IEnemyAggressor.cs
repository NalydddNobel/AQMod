using Aequus.NPCs;
using Terraria;

namespace Aequus.Common.Necromancy.Aggression {
    public interface IEnemyAggressor {
        public abstract void OnPreAI(NPC npc, AequusNPC aequus);

        public abstract void OnPostAI(NPC npc, AequusNPC aequus);
    }
}