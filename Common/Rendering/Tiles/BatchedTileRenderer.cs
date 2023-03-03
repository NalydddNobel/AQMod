using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace Aequus.Common.Rendering.Tiles;

/// <summary>
/// Grants a ModTile the ability to render post-draw elements in a batch to reduce on Begin/End calls.
/// <para>Place SpecialTileDrawing.Add(i, j, Type); into ModTile.PostDraw(...) to add it to the rendering list.</para>
/// </summary>
public interface IBatchedTile : ILoadable
{
    bool SolidLayer { get; }

    void BatchedDraw(List<BatchedTileDrawInfo> tiles, int count);
}

/// <summary>
/// Tile draw info used internally with <see cref="BatchedTileRenderer"/>.
/// </summary>
/// <param name="Tile"></param>
/// <param name="Position"></param>
public record struct BatchedTileDrawInfo(Tile Tile, Point Position);

/// <summary>
/// Special system for adding tiles with cool render effects.
/// </summary>
public class BatchedTileRenderer : ModSystem
{
    private class SpecialTileBatch
    {
        public readonly bool SolidLayer;
        public List<BatchedTileDrawInfo> Tiles;
        public IBatchedTile Drawer;
        public int Count;

        public SpecialTileBatch(IBatchedTile batchedTile)
        {
            SolidLayer = batchedTile.SolidLayer;
            Tiles = new List<BatchedTileDrawInfo>(64);
            Drawer = batchedTile;
            Count = 0;
        }
    }

    static Dictionary<int, SpecialTileBatch> _batches = new(12);

    public override void SetStaticDefaults()
    {
        foreach (var renderer in Mod.GetContent<IBatchedTile>())
        {
            if (renderer is ModTile modTile)
            {
                Register(modTile.Type, renderer);
            }
        }
    }

    public override void Unload() => _batches = null;
    public override void Load()
    {
        On.Terraria.GameContent.Drawing.TileDrawing.PreDrawTiles += TileDrawing_PreDrawTiles;
    }

    /// <summary>
    /// Registers a delegate for use with special tile drawing for selected type.
    /// </summary>
    public static void Register(int type, IBatchedTile renderer)
    {
        _batches.TryAdd(type, new SpecialTileBatch(renderer));
    }

    /// <summary>
    /// Adds the tile at position to draw buffer.
    /// </summary>
    public static void Add(int i, int j, int type)
    {
        if (_batches.TryGetValue(type, out var batch))
        {
            var info = new BatchedTileDrawInfo(Main.tile[i, j], new(i, j));
            if (batch.Tiles.Count <= batch.Count)
            {
                batch.Tiles.Add(info);
            }
            else
            {
                batch.Tiles[batch.Count] = info;
            }
            batch.Count++;
        }
    }

    public override void PostDrawTiles()
    {
        foreach (var batch in _batches.Values)
        {
            batch.Drawer.BatchedDraw(batch.Tiles, batch.Count);
        }
    }

    #region Detours
    private static void TileDrawing_PreDrawTiles(On.Terraria.GameContent.Drawing.TileDrawing.orig_PreDrawTiles orig, TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
    {
        orig(self, solidLayer, forRenderTargets, intoRenderTargets);
        if (intoRenderTargets || Lighting.UpdateEveryFrame)
        {
            foreach (var batch in _batches.Values)
            {
                if (batch.SolidLayer == solidLayer)
                {
                    batch.Count = 0;
                }
            }
        }
    }
    #endregion
}