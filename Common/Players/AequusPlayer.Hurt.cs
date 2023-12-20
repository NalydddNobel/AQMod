using Aequus.Common.Tiles.Components;
using Aequus.Core.Generator;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusPlayer {
    /// <summary>
    /// The lowest any respawn-time reducing items can go.
    /// </summary>
    public static int MinimumRespawnTime { get; set; } = 180;

    [ResetEffects]
    public int respawnTimeModifier;

    /// <summary>
    /// Sets the respawn time modifier without allowing it to stack.
    /// </summary>
    /// <param name="amount"></param>
    public void SetAccRespawnTimeModifier(int amount) {
        if (amount < 0) {
            if (respawnTimeModifier <= amount) {
                return;
            }

            respawnTimeModifier += amount;
        }
        else {
            if (respawnTimeModifier >= amount) {
                return;
            }
            respawnTimeModifier += amount;
        }
    }

    private readonly List<Point> _edgeTilesCache = new();

    private static int On_Player_GetRespawnTime(On_Player.orig_GetRespawnTime orig, Player player, bool pvp) {
        int time = orig(player, pvp);
        if (time <= MinimumRespawnTime || !player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return time;
        }
        return Math.Max(time + aequusPlayer.respawnTimeModifier, MinimumRespawnTime);
    }

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
}