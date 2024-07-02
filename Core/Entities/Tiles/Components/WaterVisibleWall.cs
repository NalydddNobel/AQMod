using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Linq;
using Terraria.Localization;
using Terraria.Map;

namespace Aequus.Core.Entities.Tiles.Components;

/// <summary>
/// Allows modded walls to be seen behind water.
/// </summary>
public interface IWaterVisibleWall {
    /// <summary>
    /// The Entry Id for when this wall is behind water.
    /// Use <see cref="WaterVisibleWall.CreateWaterEntry{T}(T, Color, LocalizedText, int)"/> to generate an entry for this automatically.
    /// <para>Defaults to 0, which uses the wall's first registered map color. (After AddRecipes, this is offset by the wall's map entry id.)</para>
    /// </summary>
    int WaterMapEntry { get; set; }
}

public class WaterVisibleWall : ModSystem {
    private const float WATER_OPACITY = 0.5f;

    public override void Load() {
        IL_MapHelper.CreateMapTile += IL_MapHelper_CreateMapTile;
    }

    public override void AddRecipes() {
        if (Main.dedServ) {
            return;
        }

        // Offset all of the water entries by the wall's entry id so we get the proper id for the water entry
        foreach (ModWall wall in Mod.GetContent<ModWall>().Where(w => w is IWaterVisibleWall)) {
            IWaterVisibleWall waterVisibleWall = wall as IWaterVisibleWall;
            int lookup = MapHelper.wallLookup[wall.Type];
            waterVisibleWall.WaterMapEntry += lookup;
        }
    }

    private static void IL_MapHelper_CreateMapTile(ILContext context) {
        var cursor = new ILCursor(context);

        // Find the tile.active method which will place the cursor right before the code for using the tile's map entry
        if (!cursor.TryGotoNext(MoveType.After, c => c.MatchCall<Tile>("active"))) {
            return;
        }

        // Find the MapHelper.GetTileBaseOption method which will place the cursor right after the code for using the tile's map entry
        if (!cursor.TryGotoNext(MoveType.After, c => c.MatchCall(typeof(MapHelper), "GetTileBaseOption"))) {
            return;
        }

        // Find the tile.invisibleWall method which will place the cursor right before the code for using the tile's wall map entry
        if (!cursor.TryGotoNext(MoveType.After, c => c.MatchCall<Tile>("invisibleWall"))) {
            return;
        }

        // Find the tile.liquidType method which will place the cursor right before the code for using the water's map entry
        if (!cursor.TryGotoNext(MoveType.After, c => c.MatchCall<Tile>("liquidType"))) {
            return;
        }

        var localVariableIndex = cursor.Instrs[cursor.Index].Operand;

        // Move to after it updates the map tile to the water map entry
        if (!cursor.TryGotoNext(MoveType.After, c => c.MatchStloc3())) {
            return;
        }

        cursor.Emit(OpCodes.Ldloc, 0);
        cursor.Emit(OpCodes.Ldloc, 3);
        cursor.Emit(OpCodes.Ldloc, localVariableIndex);
        cursor.EmitDelegate<Func<Tile, int, int, int>>((tile, mapEntry, liquidType) => {
            if (WallLoader.GetWall(tile.WallType) is IWaterVisibleWall visibleThroughWater && liquidType == LiquidID.Water) {
                return visibleThroughWater.WaterMapEntry;
            }

            return mapEntry;
        });
        cursor.Emit(OpCodes.Stloc_3);
    }

    /// <summary>
    /// Creates a map entry for the tile being covered in water.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="wall"></param>
    /// <param name="baseColor"></param>
    /// <param name="text"></param>
    /// <param name="entryNumber">Offsets the map Id lookup by this value. Defaults to 1, meaning that it assumes that this is the 2nd registered color for this wall.</param>
    public static void CreateWaterEntry<T>(T wall, Color baseColor, LocalizedText text = null, int entryNumber = 1) where T : ModWall, IWaterVisibleWall {
        wall.AddMapEntry(Color.Lerp(baseColor, CommonColor.MapWater, WATER_OPACITY), text);
        wall.WaterMapEntry = entryNumber;
    }
}
