#if !CRAB_CREVICE_DISABLE
using Aequus.Tiles.Base;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice;

public class SeaPickleTile : BaseGemTile {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        Main.tileLighted[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileNoFail[Type] = true;

        AddMapEntry(new Color(10, 82, 22), TextHelper.GetItemName<SeaPickleItem>());
        DustType = DustID.GreenMoss;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 4;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        if (Main.tile[i, j].LiquidAmount < 100) {
            r = 0.01f;
            g = 0.01f;
            b = 0.01f;
            return;
        }
        ulong seed = Helper.TileSeed(i, j);
        float wave = Math.Max(MathF.Sin(Main.GlobalTimeWrappedHourly * (0.2f + Utils.RandomFloat(ref seed) * 0.4f)), 0f);
        r = 0.3f + wave * 0.1f;
        g = 0.4f + wave * 0.15f;
        b = 0.25f + wave * 0.08f;
    }

    public static bool TryGrow(int i, int j) {
        int seaPickleTileId = ModContent.TileType<SeaPickleTile>();
        if (TileHelper.ScanTilesSquare(i, j, 10, TileHelper.HasTileAction(seaPickleTileId))) {
            return false;
        }

        int gemX = i + WorldGen.genRand.Next(-1, 2);
        int gemY = j + WorldGen.genRand.Next(-1, 2);
        if (Main.tile[gemX, gemY].HasTile || !ModContent.GetInstance<SeaPickleTile>().CanPlace(gemX, gemY)) {
            return false;
        }

        WorldGen.PlaceTile(gemX, gemY, seaPickleTileId, mute: true);
        if (Main.tile[gemX, gemY].TileType != seaPickleTileId) {
            return false;
        }

        if (Main.netMode != NetmodeID.SinglePlayer) {
            NetMessage.SendTileSquare(-1, gemX, gemY);
        }
        return true;
    }
}
#endif