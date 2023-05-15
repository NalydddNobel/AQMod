using Terraria.ModLoader;

namespace Aequus.Content.DamageClasses {
    /// <summary>
    /// Used on necromancy scepter projectiles, only procs the abilities of Magic equipment.
    /// </summary>
    public class NecromancySummonerClass : NecromancyClass {
        public override bool GetEffectInheritance(DamageClass damageClass) {
            return damageClass == Summon || damageClass == Aequus.NecromancyClass;
        }
    }
}