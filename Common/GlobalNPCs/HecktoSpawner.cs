using Aequus.NPCs.Monsters;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.GlobalNPCs {
    public class HecktoSpawner : GlobalNPC {
        public override void OnSpawn(NPC npc, IEntitySource source) {
            if (npc.type == NPCID.DungeonSpirit && Helper.HereditarySource(source, out var ent) && ent is NPC parent) {
                bool wantedValue = !Aequus.ZenithSeed;
                if (Heckto.SpawnableIDs.Contains(parent.type) == wantedValue) {
                    npc.Transform(ModContent.NPCType<Heckto>());
                }
            }
        }
    }
}