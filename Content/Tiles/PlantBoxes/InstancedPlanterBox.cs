using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Entities.Items;
using Aequus.Common.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.PlantBoxes;

internal class InstancedPlanterBox : InstancedTile, IPostSetupContent, IAddRecipes {
    public readonly ModItem Item;
    public readonly PlanterBoxInfo Info;

    public InstancedPlanterBox(string Name, string Texture, PlanterBoxInfo Info) : base(Name, Texture) {
        Item = new InstancedTileItem(this, Settings: new() {
            Value = Terraria.Item.silver,
            Research = 25,
            Texture = Texture
        });
        this.Info = Info;
    }

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

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        Vector3 rgb = Info.LightColor;
        r = rgb.X;
        g = rgb.Y;
        b = rgb.Z;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        yield return new Item(Item.Type);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Info.GlowColor != Color.Transparent) {
            Vector2 drawCoordinates = new Vector2(i, j) * 16f + Helper.TileDrawOffset - Main.screenPosition;
            Rectangle frame = new Rectangle(Main.tile[i, j].TileFrameX + 72, Main.tile[i, j].TileFrameY, 16, 16);
            spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates, frame, Info.GlowColor);
        }
    }

    public override void Load() {
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileLighted[Type] = Info.LightColor.Length() > 0f; // The Moonflower box glows
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
        AdjTiles = [TileID.PlanterBox];

        Main.RegisterItemAnimation(Item.Type, new CustomItemDrawFrame(0, 18, 32, 32));
    }

    public void PostSetupContent() {
        AutomaticallyMakeHerbsAnchorToAequusPlanterBoxes();
    }

    public void AddRecipes() {
        AutomaticallyMergeWithSupportingModdedPlanterBoxes();
    }

    void AutomaticallyMakeHerbsAnchorToAequusPlanterBoxes() {
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

    void AutomaticallyMergeWithSupportingModdedPlanterBoxes() {
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
}

public record struct PlanterBoxInfo(Condition SellCondition, Vector3 LightColor, Color GlowColor);