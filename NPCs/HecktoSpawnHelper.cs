using AQMod.NPCs.Monsters;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public class HecktoSpawnHelper : GlobalNPC
    {
        public override bool SpecialNPCLoot(NPC npc)
        {
            if (AQNPC.Sets.HecktoplasmDungeonEnemy[npc.type] && npc.lifeMax > 100)
            {
                if (npc.HasPlayerTarget && Main.hardMode && NPC.downedPlantBoss && Main.player[npc.target].ZoneDungeon)
                {
                    int spawnChance = 13;
                    if (Main.expertMode)
                        spawnChance = 9;
                    var center = npc.Center;
                    if (Main.wallDungeon[Main.tile[(int)center.X / 16, (int)center.Y / 16].wall] && Main.rand.Next(spawnChance) == 0)
                        NPC.NewNPC((int)center.X, (int)center.Y, ModContent.NPCType<Heckto>());
                }
                npc.lifeMax = 99;
                npc.NPCLoot();
                return true;
            }
            return false;
        }
    }
}