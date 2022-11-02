using Aequus.NPCs.Monsters;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.GlobalNPCs
{
    public class HecktoSpawner : GlobalNPC
    {
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.type == NPCID.DungeonSpirit && AequusHelpers.HereditarySource(source, out var ent) && ent is NPC parent)
            {
                if (Heckto.Spawnable.Contains(parent.type))
                {
                    npc.Transform(ModContent.NPCType<Heckto>());
                }
            }
        }
    }
}
