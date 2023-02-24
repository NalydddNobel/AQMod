using Aequus.Items.Placeable.Furniture.BossTrophies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture
{
    public class Trophies : ModTile
    {
        public const int OmegaStarite = 0;
        public const int Crabson = 1;
        public const int RedSprite = 2;
        public const int SpaceSquid = 3;
        public const int DustDevil = 4;
        public const int UltraStarite = 5;

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
            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            switch (frameX / 54)
            {
                case OmegaStarite:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<OmegaStariteTrophy>());
                    break;

                case Crabson:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<CrabsonTrophy>());
                    break;

                case RedSprite:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<RedSpriteTrophy>());
                    break;

                case SpaceSquid:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<RedSpriteTrophy>());
                    break;

                case DustDevil:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<DustDevilTrophy>());
                    break;

                case UltraStarite:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<UltraStariteTrophy>());
                    break;
            }
        }
    }
}
