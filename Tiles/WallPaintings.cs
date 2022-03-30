using Aequus.Items.Placeable;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles
{
    public class WallPaintings : ModTile
    {
        public const int OmegaStarite = 0;
        public const int Crabson = 1;
        public const int RedSprite = 2;
        public const int Origin = 3;
        public const int RockFromAnAlternateUniverse = 4;
        public const int OmegaStaritePainting = 5;
        public const int SpaceSquid = 6;

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
            AddMapEntry(new Color(120, 85, 60), CreateMapEntryName("Trophies"));
            AddMapEntry(new Color(120, 85, 60), CreateMapEntryName("Painting"));
        }

        public override ushort GetMapOption(int i, int j)
        {
            if (Main.tile[i, j].TileFrameX >= 162 && Main.tile[i, j].TileFrameX <= 306
                && Main.tile[i, j].TileFrameY <= 108)
            {
                return 1;
            }
            return 0;
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

                //case 2:
                //    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<RedSpriteTrophy>());
                //    break;

                case Origin:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<Origin>());
                    break;

                    //case RockFromAnAlternateUniverse:
                    //    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<RockFromAnAlternateUniverse>());
                    //    break;

                    //case OmegaStaritePainting:
                    //    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<OmegaStaritePainting>());
                    //    break;

                    //case SpaceSquid:
                    //    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<SpaceSquidTrophy>());
                    //    break;
            }
        }
    }
}
