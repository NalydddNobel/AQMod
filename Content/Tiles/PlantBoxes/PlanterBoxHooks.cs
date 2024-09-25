using Aequus.Common;
using Aequus.Common.Entities.Tiles;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Aequus.Content.Tiles.PlantBoxes;

internal class PlanterBoxHooks : LoadedType {
    protected override void Load() {
        // Planter boxes are so shoe-horned into the game that it's stupid to
        // fix every single difference between our modded planter box and vanilla ones

        //On_WorldGen.CheckAlch += CheckAlchPlanterBoxHack;
        //On_WorldGen.CheckBanner += CheckBannerPlanterBoxHack;
        //IL_WorldGen.PlantCheck += IL_WorldGen_PlantCheck;
        // Terraria only uses this in PlaceTile, but this detour may be useful if other mods use it?
        //On_WorldGen.IsFitToPlaceFlowerIn += IsFitToPlaceFlowerInPlanterBoxHack;
        //IL_WorldGen.PlaceTile += PlaceTileILEdit_AbsolutelyNotScary;

        On_Player.FigureOutWhatToPlace += On_Player_FigureOutWhatToPlace;
        IL_WorldGen.CanCutTile += IL_WorldGen_CanCutTile;
        IL_WorldGen.TryKillingReplaceableTile += IL_WorldGen_TryKillingReplaceableTile;
    }

    static void On_Player_FigureOutWhatToPlace(On_Player.orig_FigureOutWhatToPlace orig, Player self, Tile targetTile, Item sItem, out int tileToCreate, out int previewPlaceStyle, out bool? overrideCanPlace, out int? forcedRandom) {
        orig(self, targetTile, sItem, out tileToCreate, out previewPlaceStyle, out overrideCanPlace, out forcedRandom);

        if (targetTile.HasTile && TileLoader.GetTile(targetTile.TileType) is UnifiedHerb modHerb && !modHerb.IsBlooming(Player.tileTargetX, Player.tileTargetY)) {
            overrideCanPlace = false;
        }
    }

    void IL_WorldGen_TryKillingReplaceableTile(ILContext il) {
        ILCursor cursor = new ILCursor(il);

        // The first time we load the 2nd argument is right before the check for planter boxes.
        if (!cursor.TryGotoNext(i => i.MatchLdarg2())) {
            Mod.Logger.Error("Error locating ldarg2 in WorldGen.TryKillingReplaceableTile."); return;
        }

        // Grab the label for the branch statement which is actually right after ldarg2 
        ILLabel branchLabel = null;
        if (!cursor.TryGotoNext(i => i.MatchBeq(out branchLabel)) || branchLabel == null) {
            Mod.Logger.Error("Error locating beq in WorldGen.TryKillingReplaceableTile."); return;
        }

        // Increment the index, we have now fallen into the chain of type checks
        cursor.Index++;

        // Insert a check for our modded planter box in the chain of type checks.
        cursor.EmitLdarg0();
        cursor.EmitLdarg1();
        cursor.EmitDelegate((int x, int y) => TileLoader.GetTile(Main.tile[x, y + 1].TileType) is not InstancedPlanterBox);
        cursor.EmitBrfalse(branchLabel);
    }

    void IL_WorldGen_CanCutTile(ILContext il) {
        ILCursor cursor = new ILCursor(il);

        // First ldsfld is for the null check, the 2nd one is a type check.
        if (!cursor.TryGotoNext(i => i.MatchLdsflda(typeof(Main), nameof(Main.tile))) || !cursor.TryGotoNext(i => i.MatchLdsflda(typeof(Main), nameof(Main.tile)))) {
            Mod.Logger.Error("Error locating the 2nd ldsflda of Main.tile in WorldGen.CanCutTile."); return;
        }

        // Keep track of the index where the type check chain starts so we can return to it later.
        int index = cursor.Index;

        // Go forward a bit to capture the label which is used in the type checks.
        ILLabel branchLabel = null;
        if (!cursor.TryGotoNext(i => i.MatchBeq(out branchLabel)) || branchLabel == null) {
            Mod.Logger.Error("Error locating the tile check's beq in WorldGen.CanCutTile."); return;
        }

        // Set the index back now that we have the label.
        cursor.Index = index;

        // Insert a check for our modded planter box in the chain of type checks.
        cursor.EmitLdarg0();
        cursor.EmitLdarg1();
        cursor.EmitDelegate((int x, int y) => TileLoader.GetTile(Main.tile[x, y + 1].TileType) is not InstancedPlanterBox);
        cursor.EmitBrfalse(branchLabel);
    }

    private void IL_WorldGen_PlantCheck(ILContext il) {
        ILCursor cursor = new ILCursor(il);

        while (cursor.TryGotoNext(i => i.MatchLdcI4(TileID.PlanterBox))) {
            // Move down to branch statement
            cursor.Index++;

            // Push TileID.PlanterBox if type equals our modded planter box.
            cursor.EmitDelegate((int type, int checkAgainstType) => type == ModContent.TileType<InstancedPlanterBox>() ? TileID.PlanterBox : checkAgainstType);
        }
    }

    private void PlaceTileILEdit_AbsolutelyNotScary(ILContext il) {
        ILCursor cursor = new ILCursor(il);

        if (!cursor.TryGotoNext(i => i.MatchCall(typeof(WorldGen), nameof(WorldGen.IsFitToPlaceFlowerIn)))) {
            Mod.Logger.Error("Error locating WorldGen.IsFitToPlaceFlowerIn in WorldGen.PlaceTile for planter boxes.");
            return;
        }

        cursor.Index++; // Move index onto the branch operation

        // a boolean is already on the stack from the previous IsFitToPlaceFlowerIn call.
        cursor.Emit(OpCodes.Ldarg_0); // Push i
        cursor.Emit(OpCodes.Ldarg_1); // Push j
        cursor.Emit(OpCodes.Ldloc_0); // Push num (Type)

        // Check if the return value was already true, or if the type is equal to our modded planter box.
        cursor.EmitDelegate((bool returnValue, int i, int j, int type) => returnValue || type == ModContent.TileType<InstancedPlanterBox>());

        // It will now fall into the flower planting code if the type is equal to our modded planter box.

        // We now fall down into the check for the planter box type
        if (!cursor.TryGotoNext(i => i.MatchLdcI4(TileID.PlanterBox))) {
            Mod.Logger.Error("Error locating instruction for pushing TileID.PlanterBox onto the stack in WorldGen.PlaceTile.");
            return;
        }

        // Move down to branch statement
        cursor.Index++;

        // Push TileID.PlanterBox if type equals our modded planter box.
        cursor.EmitDelegate((int type, int checkAgainstType) => type == ModContent.TileType<InstancedPlanterBox>() ? TileID.PlanterBox : type);
    }

    private bool IsFitToPlaceFlowerInPlanterBoxHack(On_WorldGen.orig_IsFitToPlaceFlowerIn orig, int x, int y, int typeAttemptedToPlace) {
        return y < 1 || y > Main.maxTilesY - 1 || TileLoader.GetTile(Main.tile[x, y + 1].TileType) is not InstancedPlanterBox
            ? orig(x, y, typeAttemptedToPlace) : true;
    }

    private void CheckBannerPlanterBoxHack(On_WorldGen.orig_CheckBanner orig, int x, int j, byte type) {
        foreach (var m in Aequus.Instance.GetContent<InstancedPlanterBox>()) {
            TileID.Sets.Platforms[m.Type] = true;
        }

        orig(x, j, type);

        foreach (var m in Aequus.Instance.GetContent<InstancedPlanterBox>()) {
            TileID.Sets.Platforms[m.Type] = false;
        }
    }

    private void CheckAlchPlanterBoxHack(On_WorldGen.orig_CheckAlch orig, int x, int y) {
        Tile soil = Main.tile[x, y + 1];
        ushort soilType = soil.TileType;
        bool customPlanter = TileLoader.GetTile(soilType) is InstancedPlanterBox;
        if (customPlanter) {
            soil.TileType = TileID.PlanterBox;
        }

        orig(x, y);

        if (customPlanter) {
            soil.TileType = soilType;
        }
    }
}
