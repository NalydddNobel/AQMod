using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters
{
    public class HecktoSpawner : GlobalNPC
    {
        public static HashSet<int> HeckoSpawnable { get; private set; }

        public override void Load()
        {
            HeckoSpawnable = new HashSet<int>()
            {
                NPCID.HellArmoredBones,
                NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBonesSword,
                NPCID.DiabolistRed,
                NPCID.DiabolistWhite,
            };
        }

        public override bool SpecialOnKill(NPC npc)
        {
            if (HeckoSpawnable.Contains(npc.type) && npc.lifeMax > 100)
            {
                if (npc.HasPlayerTarget && Main.hardMode && NPC.downedPlantBoss && Main.player[npc.target].ZoneDungeon)
                {
                    int spawnChance = 13;
                    if (Main.expertMode)
                        spawnChance = 9;
                    var center = npc.Center;
                    if (Main.wallDungeon[Main.tile[(int)center.X / 16, (int)center.Y / 16].WallType] && Main.rand.Next(spawnChance) == 0)
                        NPC.NewNPC(npc.GetSource_Death("Heckto"), (int)center.X, (int)center.Y, ModContent.NPCType<Heckto>());
                }
                npc.lifeMax = 99;
                npc.NPCLoot();
                return true;
            }
            return false;
        }
    }
}