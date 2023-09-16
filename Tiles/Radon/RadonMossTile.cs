﻿using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Components;
using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Tiles.Radon;

public class RadonMossTile : ModTile, IOnPlaceTile {
    public override void SetStaticDefaults() {
        Main.tileMoss[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        AequusTile.NoVanillaRandomTickUpdates.Add(Type);
        AddMapEntry(new Color(80, 90, 90));

        DustType = DustID.Ambient_DarkBrown;
        RegisterItemDrop(ItemID.StoneBlock);
        HitSound = SoundID.Dig;

        MineResist = 3f;
        MinPick = 100;
    }

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
        //if (!effectOnly)
        //    Main.tile[i, j].TileType = TileID.Stone;
        //fail = true;
        //effectOnly = true;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        //if ((!Main.tile[i, j - 1].IsFullySolid() || !Main.tile[i, j + 1].IsFullySolid() || !Main.tile[i + 1, j].IsFullySolid() || !Main.tile[i - 1, j].IsFullySolid()) && new FastRandom(i * i + j * j * i).Next(2) == 0) {
        //    var lighting = Lighting.GetColor(i, j);
        //    if (lighting.R != 0 || lighting.G != 0 || lighting.B != 0) {
        //        RadonMossFogRenderer.Tiles.Add(new Point(i, j));
        //    }
        //}
        return true;
    }

    public override void RandomUpdate(int i, int j) {
        GrowLongMoss(i, j);
        GrowEvilPlant(i, j);
        TileHelper.SpreadGrass(i, j, TileID.Stone, ModContent.TileType<RadonMossTile>(), 1, color: Main.tile[i, j].TileColor);
        TileHelper.SpreadGrass(i, j, TileID.GrayBrick, ModContent.TileType<RadonMossBrickTile>(), 1, color: Main.tile[i, j].TileColor);
    }

    public static bool GrowEvilPlant(int i, int j) {
        int checkSize = 20;
        int plant = ModContent.TileType<RadonPlantTile>();
        var top = Main.tile[i, j - 1];
        if (top.LiquidType > 0 || top.HasTile && top.TileType != ModContent.TileType<RadonMossTile>()) {
            return false;
        }
        var rect = new Rectangle(i - checkSize, j - checkSize, checkSize * 2, checkSize * 2).Fluffize(20);
        if (!TileHelper.ScanTiles(rect, TileHelper.HasTileAction(plant), TileHelper.IsTree)) {
            if (top.TileType == ModContent.TileType<RadonMossGrass>()) {
                top.HasTile = false;
            }
            WorldGen.PlaceTile(i, j - 1, plant, mute: true);
            if (top.TileType == plant) {
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendTileSquare(-1, i - 1, j - 1, 3, 3);
                return true;
            }
        }
        return false;
    }
    public static void GrowLongMoss(int i, int j) {
        int radonMossGrass = ModContent.TileType<RadonMossGrass>();
        for (int k = -1; k <= 1; k += 2) {
            if (WorldGen.genRand.NextBool(4) && !Main.tile[i + k, j].HasTile && TileLoader.CanPlace(i + k, j, radonMossGrass)) {
                var tile = Main.tile[i + k, j];
                tile.ClearTile();
                tile.HasTile = true;
                tile.TileType = (ushort)radonMossGrass;
                tile.TileFrameY = 144; // Framing fix, so that the TileFrame method randomizes grass better
                WorldGen.TileFrame(i + k, j, resetFrame: true);
                return;
            }
        }
        for (int l = -1; l <= 1; l += 2) {
            if (WorldGen.genRand.NextBool(4) && !Main.tile[i, j + l].HasTile && TileLoader.CanPlace(i, j + l, radonMossGrass)) {
                var tile = Main.tile[i, j + l];
                tile.ClearTile();
                tile.HasTile = true;
                tile.TileType = (ushort)radonMossGrass;
                WorldGen.TileFrame(i, j + l, resetFrame: true);
                return;
            }
        }
    }

    public virtual bool? OnPlaceTile(int i, int j, bool mute, bool forced, int plr, int style) {
        if (Main.tile[i, j].TileType == TileID.GrayBrick) {
            Main.tile[i, j].TileType = (ushort)ModContent.TileType<RadonMossBrickTile>();
            WorldGen.SquareTileFrame(i, j, resetFrame: true);
            if (!mute) {
                SoundEngine.PlaySound(SoundID.Dig, new Vector2(i * 16f + 8f, j * 16f + 8f));
            }
            return true;
        }
        return null;
    }
}