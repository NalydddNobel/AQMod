using Aequus.Tiles.Furniture.Paintings.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture.Paintings {
    public class WallPaintings3x2 : ModTile {
        public const int InsurgentPainting = 0;
        public const int BongBongPainting = 1;
        public const int YinYangPainting = 2;
        public const int Fus = 3;
        public const int Ro = 4;
        public const int DAH = 5;
        public const int OliverPainting = 6;

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            DustType = DustID.WoodFurniture;
            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Painting"));
        }
    }
}