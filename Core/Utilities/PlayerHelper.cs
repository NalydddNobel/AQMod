using Aequus.Content.WorldEvents.DemonSiege;
using Aequus.Content.WorldEvents.Glimmer;
using Aequus.Content.WorldEvents.SpaceStorm;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus;

public static class PlayerHelper {
    #region Biomes
    public static bool ZoneGlimmer(this Player player) {
        return player.InModBiome<GlimmerZone>();
    }
    public static bool ZoneDemonSiege(this Player player) {
        return player.InModBiome<DemonSiegeZone>();
    }
    public static bool ZoneSpaceStorm(this Player player) {
        return player.InModBiome<SpaceStormZone>();
    }
    #endregion

    #region Damage & Crit
    /// <summary>
    /// Checks weird damage restrictions for NPCs. This literally only checks for Fairies as of right now.
    /// </summary>
    /// <param name="npc"></param>
    /// <returns>Whether the enemy should be immune to this hit (true = Do not hit)</returns>
    public static bool WeirdNPCHitRestrictions(NPC npc) {
        return npc.aiStyle == NPCAIStyleID.Fairy && !(npc.ai[2] <= 1f);
    }

    public static bool RollCrit<T>(this Player player) where T : DamageClass {
        return RollCrit(player, ModContent.GetInstance<T>());
    }
    public static bool RollCrit(this Player player, DamageClass damageClass) {
        return !damageClass.UseStandardCritCalcs ? false : Main.rand.Next(100) < player.GetTotalCritChance(damageClass);
    }
    #endregion
}