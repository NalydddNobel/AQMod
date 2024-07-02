using Aequus.Core.ContentGeneration;
using Aequus.Core.Entities.Tiles.Components;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ObjectData;
using tModLoaderExtended.Terraria.GameContent.Creative;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequus.Old.Content.Tiles.Herbs.PlanterBoxes;

public sealed class PlanterBox : ModTile, IRandomUpdateOverride, IPostSetupContent, IAddRecipes {
    public const int STYLE_MORAY = 0;
    public const int STYLE_MISTRAL = 1;
    public const int STYLE_MANACLE = 2;
    public const int STYLE_MOONFLOWER = 3;
    public const int STYLE_COUNT = 4;

    public List<PlanterBoxItemInfo> RegisteredPlanterBoxItems { get; private set; } = new();

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        if (!WorldGen.InWorld(i, j, 5)) {
            return base.TileFrame(i, j, ref resetFrame, ref noBreak);
        }

        Tile left = Main.tile[i - 1, j];
        Tile right = Main.tile[i + 1, j];
        Tile tile = Main.tile[i, j];
        bool[] merge = Main.tileMerge[Type];
        bool mergeLeft = false;
        bool mergeRight = false;

        if (left.HasTile) {
            mergeLeft = merge[left.TileType];
        }
        if (right.HasTile) {
            mergeRight = merge[right.TileType];
        }

        if (mergeLeft && mergeRight) {
            tile.TileFrameX = 18;
        }
        else if (mergeLeft) {
            tile.TileFrameX = 36;
        }
        else if (mergeRight) {
            tile.TileFrameX = 0;
        }
        else {
            tile.TileFrameX = 54;
        }

        return base.TileFrame(i, j, ref resetFrame, ref noBreak);
    }

    //public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
    //    Tile tile = Main.tile[i, j];
    //    tile.TileType = TileID.PlanterBox;

    //    if (!_loop) {
    //        _loop = true;

    //        try {
    //            WorldGen.TileFrame(i, j, resetFrame, noBreak);
    //        }
    //        catch {
    //        }

    //        _loop = false;
    //    }

    //    // Reframe the other two tiles next to us
    //    if (resetFrame && i > 5 && i < Main.maxTilesX - 5) {
    //        WorldGen.TileFrame(i - 1, j, false, true);
    //        WorldGen.TileFrame(i + 1, j, false, true);
    //    }

    //    tile.TileType = Type;

    //    return true;
    //}

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 6 : 3;
    }

    public override bool Slope(int i, int j) {
        return false;
    }

    // Cause place style just changes the X coordinate, even though this is styled through the Y coordinate
    public override void PlaceInWorld(int i, int j, Item item) {
        Main.tile[i, j].TileFrameY = (short)(item.placeStyle * 18);

        if (Main.netMode == NetmodeID.MultiplayerClient) {
            NetMessage.SendTileSquare(-1, i, j);
        }
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        switch (Main.tile[i, j].TileFrameY / 18) {
            case STYLE_MOONFLOWER:
                // RGB lighting for Meteor Bricks
                r = 0.32f;
                g = 0.16f;
                b = 0.12f;
                break;
        }
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        int style = Math.Clamp(Main.tile[i, j].TileFrameY / 18, 0, STYLE_COUNT);

        yield return new Item(RegisteredPlanterBoxItems[style].ModItem.Type, stack: 1);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        switch (Main.tile[i, j].TileFrameY / 18) {
            case STYLE_MOONFLOWER:
                // Glow mask color for meteorite furniture
                // TileDrawing._meteorColor
                Color meteorColor = new Color(100, 100, 100, 0);

                spriteBatch.Draw(AequusTextures.PlanterBoxGlow, new Vector2(i, j) * 16f + TileHelper.DrawOffset - Main.screenPosition, new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16), meteorColor);
                break;
        }
    }

    //bool IRandomUpdateOverride.PreRandomUpdate(int i, int j) {
    //    Main.tile[i, j].TileType = TileID.PlanterBox;
    //    return false;
    //}

    //void IRandomUpdateOverride.PostRandomUpdate(int i, int j) {
    //    Main.tile[i, j].TileType = Type;
    //}

    public override void Load() {
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

        AddItem("Moray", ItemID.BlinkrootPlanterBox, AequusSystem.ConditionDownedSalamancer);
        AddItem("Mistral", ItemID.FireBlossomPlanterBox, AequusSystem.ConditionDownedDustDevil);
        AddItem("Manacle", ItemID.ShiverthornPlanterBox, AequusSystem.ConditionDownedDemonSiege);
        AddItem("Moonflower", ItemID.CrimsonPlanterBox, AequusSystem.ConditionDownedOmegaStarite);

        void AddItem(string name, int shopSortItemIdTarget, Condition sellCondition) {
            InstancedTileItem planterBoxItem = new InstancedTileItem(this, style: RegisteredPlanterBoxItems.Count, name, rarity: ItemRarityID.White, value: Item.silver, researchSacrificeCount: 25, journeyOverride: new JourneySortByTileId(TileID.PlanterBox));

            Mod.AddContent(planterBoxItem);

            RegisteredPlanterBoxItems.Add(new PlanterBoxItemInfo(planterBoxItem, shopSortItemIdTarget, sellCondition));
        }
    }

    public override void SetStaticDefaults() {
        Main.tileLighted[Type] = true; // The Moonflower box glows
        Main.tileFrameImportant[Type] = true;
        Main.tileTable[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileSolidTop[Type] = true;

        //Main.tileMerge[Type][TileID.PlanterBox] = true;
        //Main.tileMerge[TileID.PlanterBox][Type] = true;

        // Dumb hack, but this self-merge means that we dont need to check tileMerge
        // AND if the type is equal to our modded planter box type, we can just check tileMerge.
        Main.tileMerge[Type][Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.DoesntGetReplacedWithTileReplacement[Type] = true;
        TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;

        //TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        //TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
        //TileObjectData.newTile.AnchorRight = AnchorData.Empty;
        //TileObjectData.newTile.AnchorTop = AnchorData.Empty;
        //TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
        //TileObjectData.newTile.LavaDeath = false;
        //TileObjectData.newTile.StyleHorizontal = false;
        //TileObjectData.addTile(Type);

        AddMapEntry(CommonColor.MapWoodFurniture);
        DustType = 37;
        AdjTiles = new int[] { TileID.PlanterBox };
    }

    public void PostSetupContent(Mod mod) {
        AutomaticallyMakeHerbsAnchorToAequusPlanterBoxes();
    }

    public void AddRecipes(Mod mod) {
        AutomaticallyMergeWithSupportingModdedPlanterBoxes();
    }

    private void AutomaticallyMakeHerbsAnchorToAequusPlanterBoxes() {
        for (int i = 0; i < TileLoader.TileCount; i++) {
            TileObjectData objData = TileObjectData.GetTileData(i, 0);
            if (objData == null || objData.AnchorAlternateTiles == null || objData.AnchorAlternateTiles.Length == 0) {
                continue;
            }

            // Check if this tile anchors to Planter Boxes
            if (objData.AnchorAlternateTiles.Any(tileId => tileId == TileID.PlanterBox)) {
                lock (objData) {
                    // If so, add Aequus' planter box automatically.
                    int[] anchorAlternates = objData.AnchorAlternateTiles;
                    Array.Resize(ref anchorAlternates, anchorAlternates.Length + 1);
                    anchorAlternates[^1] = Type;
                    objData.AnchorAlternateTiles = anchorAlternates;
                }
            }
        }
    }

    private void AutomaticallyMergeWithSupportingModdedPlanterBoxes() {
        for (int i = TileID.Count; i < TileLoader.TileCount; i++) {
            ModTile modTile = TileLoader.GetTile(i);

            if (modTile == null || modTile.AdjTiles == null || modTile.AdjTiles.Length == 0 || !Main.tileTable[i] || !Main.tileSolidTop[i]) {
                continue;
            }

            // Other mods which populate their AdjTile array with TileID.PlanterBox will be automatically merged with
            // AdjTiles is a very convenient way to detect if a tile is a counterpart of a vanilla type,
            // even if planter boxes are not used for crafting.
            if (modTile.AdjTiles.Any(tileId => tileId == TileID.PlanterBox)) {
                Main.tileMerge[Type][i] = true;
            }
        }
    }

    #region Hooks
    private static void On_Player_FigureOutWhatToPlace(On_Player.orig_FigureOutWhatToPlace orig, Player self, Tile targetTile, Item sItem, out int tileToCreate, out int previewPlaceStyle, out bool? overrideCanPlace, out int? forcedRandom) {
        orig(self, targetTile, sItem, out tileToCreate, out previewPlaceStyle, out overrideCanPlace, out forcedRandom);

        if (targetTile.HasTile && TileLoader.GetTile(targetTile.TileType) is ModHerb modHerb && modHerb.GetGrowthStage(Player.tileTargetX, Player.tileTargetY) != ModHerb.STAGE_BLOOMING) {
            overrideCanPlace = false;
        }
    }

    private void IL_WorldGen_TryKillingReplaceableTile(ILContext il) {
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
        cursor.EmitDelegate((int x, int y) => Main.tile[x, y + 1].TileType != ModContent.TileType<PlanterBox>());
        cursor.EmitBrfalse(branchLabel);
    }

    private void IL_WorldGen_CanCutTile(ILContext il) {
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
        cursor.EmitDelegate((int x, int y) => Main.tile[x, y + 1].TileType != ModContent.TileType<PlanterBox>());
        cursor.EmitBrfalse(branchLabel);
    }

    private void IL_WorldGen_PlantCheck(ILContext il) {
        ILCursor cursor = new ILCursor(il);

        while (cursor.TryGotoNext(i => i.MatchLdcI4(TileID.PlanterBox))) {
            // Move down to branch statement
            cursor.Index++;

            // Push TileID.PlanterBox if type equals our modded planter box.
            cursor.EmitDelegate((int type, int checkAgainstType) => type == ModContent.TileType<PlanterBox>() ? TileID.PlanterBox : checkAgainstType);
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
        cursor.EmitDelegate((bool returnValue, int i, int j, int type) => returnValue || type == ModContent.TileType<PlanterBox>());

        // It will now fall into the flower planting code if the type is equal to our modded planter box.

        // We now fall down into the check for the planter box type
        if (!cursor.TryGotoNext(i => i.MatchLdcI4(TileID.PlanterBox))) {
            Mod.Logger.Error("Error locating instruction for pushing TileID.PlanterBox onto the stack in WorldGen.PlaceTile.");
            return;
        }

        // Move down to branch statement
        cursor.Index++;

        // Push TileID.PlanterBox if type equals our modded planter box.
        cursor.EmitDelegate((int type, int checkAgainstType) => type == ModContent.TileType<PlanterBox>() ? TileID.PlanterBox : type);
    }

    private bool IsFitToPlaceFlowerInPlanterBoxHack(On_WorldGen.orig_IsFitToPlaceFlowerIn orig, int x, int y, int typeAttemptedToPlace) {
        return y < 1 || y > Main.maxTilesY - 1 || Main.tile[x, y + 1].TileType != Type
            ? orig(x, y, typeAttemptedToPlace) : true;
    }

    private void CheckBannerPlanterBoxHack(On_WorldGen.orig_CheckBanner orig, int x, int j, byte type) {
        TileID.Sets.Platforms[Type] = true;
        orig(x, j, type);
        TileID.Sets.Platforms[Type] = false;
    }

    private void CheckAlchPlanterBoxHack(On_WorldGen.orig_CheckAlch orig, int x, int y) {
        Tile soil = Main.tile[x, y + 1];
        ushort soilType = soil.TileType;
        if (soilType == Type) {
            soil.TileType = TileID.PlanterBox;
        }

        orig(x, y);

        if (soilType == Type) {
            soil.TileType = soilType;
        }
    }
    #endregion

    public record struct PlanterBoxItemInfo(ModItem ModItem, int ShopSortingItemIdTarget, Condition SellCondition);
}
