using Aequus.Core.ContentGeneration;
using Aequus.Core.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ObjectData;
using Terraria.ID;

namespace Aequus.Old.Content.Tiles.Herbs.PlanterBoxes;

public sealed class PlanterBox : ModTile, IPostSetupContent, IAddRecipes {
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

    public override void Load() {
        AddItem("Moray", ItemID.BlinkrootPlanterBox, Aequus.ConditionDownedAquaticBoss);
        AddItem("Mistral", ItemID.FireBlossomPlanterBox, Aequus.ConditionDownedTrueAtmosphereBoss);
        AddItem("Manacle", ItemID.ShiverthornPlanterBox, Aequus.ConditionDownedDemonBoss);
        AddItem("Moonflower", ItemID.CrimsonPlanterBox, Aequus.ConditionDownedTrueCosmicBoss);

        void AddItem(string name, int shopSortItemIdTarget, Condition sellCondition) {
            InstancedTileItem planterBoxItem = new InstancedTileItem(this, style: RegisteredPlanterBoxItems.Count, name, rarity: ItemRarityID.White, value: Item.silver, researchSacrificeCount: 25);

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
        Main.tileMerge[Type][TileID.PlanterBox] = true;
        Main.tileMerge[TileID.PlanterBox][Type] = true;

        // Dumb hack, but this self-merge means that we dont need to check tileMerge
        // AND if the type is equal to our modded planter box type, we can just check tileMerge.
        Main.tileMerge[Type][Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;

        //TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        //TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
        //TileObjectData.newTile.AnchorRight = AnchorData.Empty;
        //TileObjectData.newTile.AnchorTop = AnchorData.Empty;
        //TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
        //TileObjectData.newTile.LavaDeath = false;
        //TileObjectData.newTile.StyleHorizontal = false;
        //TileObjectData.addTile(Type);

        AddMapEntry(CommonColor.TILE_FURNITURE);
        DustType = 37;
        AdjTiles = new int[] { TileID.PlanterBox };
    }

    public void PostSetupContent(Aequus aequus) {
        AutomaticallyMakeHerbsAnchorToAequusPlanterBoxes();
    }

    public void AddRecipes(Aequus aequus) {
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

    public record struct PlanterBoxItemInfo(ModItem ModItem, int ShopSortingItemIdTarget, Condition SellCondition);
}
