using Aequus.NPCs;
using Terraria;

namespace Aequus.Projectiles.Summon
{
    public class NecromanticEnemySpawner : RevenantEnemySpawner
    {
        protected override void OnSpawnZombie(NPC npc, NecromancyNPC zombie)
        {
            base.OnSpawnZombie(npc, zombie);
            zombie.slotsConsumed *= 2;
        }
    }
}