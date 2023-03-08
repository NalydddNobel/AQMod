using Aequus.Content.Boss.Crabson.Misc;
using Aequus.Items.Materials;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice
{
    public class PearlsTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileShine[Type] = 400;
            Main.tileShine2[Type] = true;
            Main.tileOreFinderPriority[Type] = 110;
            Main.tileSpelunker[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

            AddMapEntry(new Color(190, 200, 222), TextHelper.GetText("MapObject.Pearl"));
            AddMapEntry(new Color(105, 186, 220), TextHelper.GetText("MapObject.HypnoticPearl"));
            DustType = DustID.Glass;
            ItemDrop = ModContent.ItemType<PearlShardWhite>();
            HitSound = SoundID.Shatter;
        }

        public override ushort GetMapOption(int i, int j)
        {
            return (ushort)(Main.tile[i, j].TileFrameX / 18 == 3 ? 1 : 0);
        }

        public override bool CanPlace(int i, int j)
        {
            var top = Framing.GetTileSafely(i, j - 1);
            if (top.HasTile && !top.BottomSlope && top.TileType >= 0 && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType])
            {
                return true;
            }
            var bottom = Framing.GetTileSafely(i, j + 1);
            if (bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType]))
            {
                return true;
            }
            var left = Framing.GetTileSafely(i - 1, j);
            if (left.HasTile && left.TileType >= 0 && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType])
            {
                return true;
            }
            var right = Framing.GetTileSafely(i + 1, j);
            if (right.HasTile && right.TileType >= 0 && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType])
            {
                return true;
            }
            return false;
        }

        public override bool Drop(int i, int j)
        {
            switch (Main.tile[i, j].TileFrameX / 18)
            {
                case 0:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<PearlShardWhite>(), Main.rand.Next(2) + 1);
                    return false;
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<PearlShardBlack>(), Main.rand.Next(2) + 1);
                    return false;
                case 2:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<PearlShardPink>(), Main.rand.Next(2) + 1);
                    return false;
                case 3:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<HypnoticPearl>());
                    return false;
            }
            return true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.12f;
            g = 0.12f;
            b = 0.12f;
            if (Main.tile[i, j].TileFrameX >= 18 * 3)
            {
                g += (float)Math.Sin(Main.GameUpdateCount / 30f) * 0.33f;
                b += (float)Math.Sin(Main.GameUpdateCount / 30f + MathHelper.Pi) * 0.33f;
                if (g < 0.3f)
                    g = 0.3f;
                if (b < 0.3f)
                    b = 0.3f;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            AequusTile.GemFrame(i, j);
            return false;
        }
    }
}