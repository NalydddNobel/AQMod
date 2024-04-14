using Aequus.Common.Tiles.Components;
using Aequus.Core.CodeGeneration;
using Aequus.Old.Content.StatusEffects;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus;

public partial class AequusPlayer {
    /// <summary>The lowest any respawn-time reducing items can go.</summary>
    public static int MinimumRespawnTime { get; set; } = 180;

    [ResetEffects]
    public int respawnTimeModifierFlat;

    /// <summary>
    /// Sets the respawn time modifier without allowing it to stack.
    /// </summary>
    /// <param name="amount"></param>
    public void SetNonStackingRespawnTimeModifier(int amount) {
        if (amount < 0) {
            if (respawnTimeModifierFlat <= amount) {
                return;
            }

            respawnTimeModifierFlat += amount;
        }
        else {
            if (respawnTimeModifierFlat >= amount) {
                return;
            }
            respawnTimeModifierFlat += amount;
        }
    }

    private readonly List<Point> _edgeTilesCache = new();

    private void HandleTileEffects() {
        _edgeTilesCache.Clear();
        Collision.GetEntityEdgeTiles(_edgeTilesCache, Player);
        foreach (Point touchedTile in _edgeTilesCache) {
            Tile tile = Framing.GetTileSafely(touchedTile);
            if (!tile.HasUnactuatedTile) {
                continue;
            }

            if (TileLoader.GetTile(tile.TileType) is ITouchEffects touchEffects) {
                touchEffects.Touch(touchedTile.X, touchedTile.Y, Player, this);
            }
        }
    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource) {
        EditDeathMessage(damageSource);
        return true;
    }

    #region Death Messages
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for Fall Damage.</summary>
    public const int DEATH_FALLING = 0;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for Drowning.</summary>
    public const int DEATH_DROWNED = 1;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for Lava Damage.</summary>
    public const int DEATH_LAVA = 2;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for Misc Deaths. Used by:
    /// <list type="bullet">
    /// <item>Cactus in DST worlds.</item>
    /// <item>Tiles in <see cref="TileID.Sets.TouchDamageImmediate"/></item>
    /// </list>
    /// </summary>
    public const int DEATH_DEFAULT = 3;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for Misc Deaths. Used by:
    /// <list type="bullet">
    /// <item>Hammering a Demon Altar in Pre-Hardmode.</item>
    /// </list>
    /// </summary>
    public const int DEATH_WAS_SLAIN_1 = 4;
    /// <summary>Seemingly unused, emits the same text as <see cref="DEATH_WAS_SLAIN_1"/>.</summary>
    public const int DEATH_WAS_SLAIN_2 = 255;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying while petrified (<see cref="Player.stoned"/>).</summary>
    public const int DEATH_PETRIFIED = 5;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to the Companion Cube's stab.</summary>
    public const int DEATH_COMPANION_CUBE_STAB = 6;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to suffocation damage (<see cref="Player.suffocating"/>).</summary>
    public const int DEATH_SUFFOCATING = 7;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to fire damage. Is also the fallback death message for dying to any Damage Over Time effect.</summary>
    public const int DEATH_BURNING = 8;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to poison or venom damage. (<see cref="Player.poisoned"/>, <see cref="Player.venom"/>)</summary>
    public const int DEATH_POISON = 9;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to electrified damage. (<see cref="Player.electrified"/>)</summary>
    public const int DEATH_ELECTRIFIED = 10;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for trying to escape Wall of Flesh.</summary>
    public const int DEATH_WALL_OF_FLESH_ESCAPE = 11;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for being tongued by Wall of Flesh. (<see cref="Player.tongued"/>)</summary>
    public const int DEATH_WALL_OF_FLESH_LICK = 12;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to Chaos State. These are genderless lines which have a 50% chance of appearing instead of the gendered lines. (<see cref="Player.chaosState"/>)</summary>
    public const int DEATH_CHAOS_STATE_GENDERLESS_LINES = 13;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to Chaos State. These are male lines which have a 50% chance of appearing instead of the genderless lines. (<see cref="Player.chaosState"/>)</summary>
    public const int DEATH_CHAOS_STATE_MALE_LINES = 14;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to Chaos State. These are female lines which have a 50% chance of appearing instead of the genderless lines. (<see cref="Player.chaosState"/>)</summary>
    public const int DEATH_CHAOS_STATE_FEMALE_LINES = 15;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to a hostile player's Inferno Potion effect. (<see cref="Player.inferno"/>)</summary>
    public const int DEATH_HOSTILE_PLAYER_INFERNO_POTION = 16;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to darkness in the DST seed. (<see cref="Terraria.GameContent.DontStarveDarknessDamageDealer"/>)</summary>
    public const int DEATH_DST_DARKNESS = 17;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying to starvation in the DST seed. (<see cref="Player.starving"/>)</summary>
    public const int DEATH_DST_STARVING = 18;
    /// <summary><see cref="PlayerDeathReason.SourceOtherIndex"/> for dying from going offscreen to space. (<see cref="Main.remixWorld"/>, <see cref="Player.forcedGravity"/>)</summary>
    public const int DEATH_REMIX_SPACE = 19;
    public const int DEATH_EMPTY = 254;

    private void EditDeathMessage(PlayerDeathReason damageSource) {
        switch (damageSource.SourceOtherIndex) {
            case DEATH_FALLING:
                // Fall damage with Weighted Horseshoe.
                if (accWeightedHorseshoe != null && Main.rand.NextBool()) {
                    SetReason("WeightedHorseshoe", 3);
                }
                break;

            case DEATH_BURNING:
            case DEATH_POISON:
            case DEATH_DST_STARVING:
                // Dying of custom DoTs.
                if (Player.HasBuff(ModContent.BuffType<BlueFire>())) {
                    SetReason("ManaFire", 4);
                }
                break;
        }

        void SetReason(string reason, int variants = 1) {
            SignCustomDeathReason(damageSource, reason, variants);
        }
    }

    public static PlayerDeathReason CustomDeathReason(string reason, int variants = 1) {
        PlayerDeathReason deathReason = new PlayerDeathReason();
        SignCustomDeathReason(deathReason, reason, variants);
        return deathReason;
    }

    private static void SignCustomDeathReason(PlayerDeathReason damageSource, string reason, int variants = 1) {
        damageSource.SourceCustomReason = $"Mods.Aequus.Player.DeathMessage.{reason}.{(variants > 1 ? Main.rand.Next(variants) : "")}";
    }
    #endregion
}