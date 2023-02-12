using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.CraftingStation
{
    public class ArmorSynthesizerTile : ModTile
    {
        public override string Texture => Aequus.PlaceholderFurniture;

        public override void SetStaticDefaults()
        {
            Main.tileHammer[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock, };
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.addTile(Type);
            DustType = DustID.Iron;
            AddMapEntry(new Color(175, 15, 15), CreateMapEntryName());
        }
    }
}