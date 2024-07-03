using System.Collections.Generic;

namespace Aequu2.Core.Graphics.Tiles;

/// <summary>
/// Tile draw info used internally with <see cref="BatchedTileRenderer"/>.
/// </summary>
/// <param name="Tile"></param>
/// <param name="Position"></param>
public record struct BatchedTileDrawInfo(Tile Tile, Point Position);

/// <summary>
/// Special system for adding tiles with cool render effects.
/// </summary>
public sealed class BatchedTileRenderer : ModSystem {
    internal class SpecialTileBatch {
        public readonly bool SolidLayer;
        public List<BatchedTileDrawInfo> Tiles;
        public IBatchedTile Drawer;
        public int Count;

        public SpecialTileBatch(IBatchedTile batchedTile) {
            SolidLayer = batchedTile.SolidLayerTile;
            Tiles = new List<BatchedTileDrawInfo>(64);
            Drawer = batchedTile;
            Count = 0;
        }
    }

    internal static Dictionary<int, SpecialTileBatch> _batches = new(12);

    public override void SetStaticDefaults() {
        foreach (var renderer in Mod.GetContent<IBatchedTile>()) {
            if (renderer is ModTile modTile) {
                Register(modTile.Type, renderer);
            }
        }
    }

    public override void Unload() {
        _batches = null;
    }

    public override void Load() {
    }

    /// <summary>
    /// Registers a delegate for use with special tile drawing for selected type.
    /// </summary>
    public static void Register(int type, IBatchedTile renderer) {
        _batches.TryAdd(type, new SpecialTileBatch(renderer));
    }

    /// <summary>
    /// Adds the tile at position to draw buffer.
    /// </summary>
    public static void Add(int i, int j, int type) {
        if (_batches.TryGetValue(type, out var batch)) {
            var info = new BatchedTileDrawInfo(Main.tile[i, j], new(i, j));
            if (batch.Tiles.Count <= batch.Count) {
                batch.Tiles.Add(info);
            }
            else {
                batch.Tiles[batch.Count] = info;
            }
            batch.Count++;
        }
    }

    public override void PostDrawTiles() {
        foreach (var batch in _batches.Values) {
            batch.Drawer.BatchedPostDraw(batch.Tiles, batch.Count);
        }
    }
}