﻿using System;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture.Jeweled;
public class JeweledCandelabra : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<JeweledCandelabraTile>());
        Item.width = 16;
        Item.height = 24;
        Item.rare = ItemRarityID.White;
        Item.value = Item.buyPrice(gold: 1);
        Item.maxStack = Item.CommonMaxStack;
    }
}

public class JeweledCandelabraTile : ModTile {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileShine[Type] = 5000;
        Main.tileShine2[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.addTile(Type);
        AddMapEntry(Color.Gold * 1.25f, TextHelper.GetText("ItemName.JeweledCandelabra"));
        HitSound = SoundID.Dig;
        DustType = DustID.Torch;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.8f;
        g = 0.66f;
        b = 0.33f;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY) {
        Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<JeweledCandelabra>());
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX == 18 && Main.tile[i, j].TileFrameY % 36 >= 18) {
            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
            var drawCoordinates = new Vector2((i - 1) * 16f, (j - 1) * 16f) + Helper.TileDrawOffset - Main.screenPosition;
            var texture = AequusTextures.JeweledCandelabraTile_Flame;
            var origin = texture.Size() / 2f;
            drawCoordinates += new Vector2(15f, 13f);
            var frame = texture.Bounds();
            for (int k = 0; k < 7; k++) {
                var scale = new Vector2(1f, 1f + Utils.RandomInt(ref randSeed, 0, 10) * 0.02f + Helper.Wave(k / 14f * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 2.5f, 0f, 0.5f));
                Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(Utils.RandomInt(ref randSeed, -10, 11) * 0.15f, Utils.RandomInt(ref randSeed, -10, 1) * 0.35f - (scale.Y - 1f)),
                    frame, new Color(110 + Utils.RandomInt(ref randSeed, -10, 50), 40 + Math.Max(Utils.RandomInt(ref randSeed, -10, 60),
                    (int)Helper.Wave(Main.GlobalTimeWrappedHourly * 5f + k * 0.025f + Utils.RandomInt(ref randSeed, -10, 10) * 0.1f, -10f, 60f)), 10, 0) * 0.7f, Utils.RandomInt(ref randSeed, -20, 20) * 0.01f * scale.Y, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}