using Aequus.Content.WorldEvents.DemonSiege;
using Aequus.Content.WorldEvents.Glimmer;
using Aequus.Content.WorldEvents.SpaceStorm;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus;

public static class PlayerHelper {
    /// <summary>
    /// Gets the "Player Focus". This is by default the player's centre, but when using the Drone, this returns the drone's position.
    /// <para>This position is used to make Radon Fog disappear when approched by the player, or by their controlled drone.</para>
    /// <para>This position also ignores camera panning effects, like screen shakes, scoping, ect.</para>
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static Vector2 GetPlayerFocusPosition(this Player player) {
        if (Main.DroneCameraTracker.TryTracking(out var dronePosition)) {
            return dronePosition;
        }

        return player.Center;
    }

    public static bool IsFalling(this Player player) {
        return Helper.IsFalling(player.velocity, player.gravDir);
    }

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