﻿using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Radon;

public class RadonMossGrass : ModTile {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileCut[Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.UsesCustomCanPlace = false;
        TileObjectData.newTile.CoordinateHeights = new int[1] { 22 };
        TileObjectData.newTile.CoordinateWidth = 22;
        TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
        TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
        TileObjectData.newTile.AnchorRight = AnchorData.Empty;
        TileObjectData.newTile.AnchorTop = AnchorData.Empty;
        TileObjectData.newTile.DrawYOffset = -4;
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(55, 65, 65));
        HitSound = SoundID.Grass;
        DustType = DustID.Ambient_DarkBrown;
    }

    // These are for "Smart Interact" (right click effects), not smart cursor, dumb naly
    //public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
    //    var heldItem = settings.player.HeldItem;
    //    if (!heldItem.IsAir && ItemID.Sets.IsPaintScraper[heldItem.type]) {
    //        return true;
    //    }
    //    return false;
    //}

    //public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY) {
    //    width = 1;
    //    height = 1;
    //}

    public override bool CanPlace(int i, int j) {
        return TileHelper.GetGemFramingAnchor(i, j, RadonMossTile.IsRadonMoss).IsSolidTileAnchor();
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset) {
        frameXOffset += 1;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        TileHelper.GemFraming(i, j, RadonMossTile.IsRadonMoss);
        return false;
    }
}