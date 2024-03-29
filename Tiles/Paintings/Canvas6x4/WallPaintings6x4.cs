﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Paintings.Canvas6x4 {
    public class WallPaintings6x4 : ModTile {
        public const int HomeworldPainting = 0;
        public const int BreadRoachPainting = 1;
        public const int XmasYinYangPainting = 2;
        public const int OmegaStaritePainting = 3;

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.newTile.Origin = new Point16(3, 3);
            TileObjectData.addTile(Type);
            DustType = DustID.WoodFurniture;
            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Painting"));
        }
    }
}