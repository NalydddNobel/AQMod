using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.NPCs.OccultistNPC.Shop;
using Aequus.NPCs.PhysicistNPC.Shop;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture
{
    //[LegacyName("WallPaintings")]
    public class WallPaintings : ModTile
    {
        public const int OriginPainting = 0;
        public const int RockFromAnAlternateUniversePainting = 1;
        public const int OmegaStaritePainting = 2;
        public const int GoreNestPainting = 3;
        public const int ExLydSpacePainting = 4;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            DustType = DustID.WoodFurniture;
            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Painting"));

        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            switch (frameX / 54)
            {
                case OriginPainting:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<OriginPainting>());
                    break;
                case RockFromAnAlternateUniversePainting:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<CatalystPainting>());
                    break;
                case GoreNestPainting:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<GoreNestPainting>());
                    break;
                case ExLydSpacePainting:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<ExLydSpacePainting>());
                    break;
            }
        }
    }
}