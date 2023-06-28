using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice {
    [LegacyName("SeaPickle")]
    public class SeaPickleItem : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<SeaPickleTile>());
            Item.value = Item.sellPrice(copper: 1);
        }
    }

    public class SeaPickleTile : ModTile {
        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileNoFail[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;

            AddMapEntry(new Color(10, 82, 22), TextHelper.GetItemName<SeaPickleItem>());
            DustType = DustID.GreenMoss;
        }

        public override bool CanPlace(int i, int j) {
            var top = Framing.GetTileSafely(i, j - 1);
            if (top.HasTile && !top.BottomSlope && top.TileType >= 0 && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType]) {
                return true;
            }
            var bottom = Framing.GetTileSafely(i, j + 1);
            if (bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType])) {
                return true;
            }
            var left = Framing.GetTileSafely(i - 1, j);
            if (left.HasTile && left.TileType >= 0 && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType]) {
                return true;
            }
            var right = Framing.GetTileSafely(i + 1, j);
            if (right.HasTile && right.TileType >= 0 && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType]) {
                return true;
            }
            return false;
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

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
            TileHelper.Frames.GemFraming(i, j);
            return false;
        }

        public static bool TryGrow(int i, int j) {
            int seaPickleTileId = ModContent.TileType<SeaPickleTile>();
            if (!TileHelper.ScanTilesSquare(i, j, 10, TileHelper.HasTileAction(seaPickleTileId))) {
                return false;
            }

            int gemX = i + WorldGen.genRand.Next(-1, 2);
            int gemY = j + WorldGen.genRand.Next(-1, 2);
            if (!Main.tile[gemX, gemY].HasTile && !ModContent.GetInstance<SeaPickleTile>().CanPlace(gemX, gemY)) {
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
}