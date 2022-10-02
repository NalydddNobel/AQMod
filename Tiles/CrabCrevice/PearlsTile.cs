﻿using Aequus.Items.Boss.Summons;
using Microsoft.Xna.Framework;
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

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;

            AddMapEntry(new Color(10, 82, 22), CreateMapEntryName("Pearl"));
            DustType = DustID.Glass;
            ItemDrop = ItemID.WhitePearl;
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
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ItemID.BlackPearl);
                    return false;
                case 2:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ItemID.PinkPearl);
                    return false;
                case 3:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<HypnoticPearl>());
                    return false;
            }
            return true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            AequusTile.GemFrame(i, j);
            return false;
        }
    }
}