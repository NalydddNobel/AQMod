using AQMod.Items.BossItems;
using AQMod.Items.BossItems.Crabson;
using AQMod.Items.BossItems.Starite;
using AQMod.Items.Placeable.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Furniture
{
    public class Trophies : ModTile
    {
        public const int OmegaStarite = 0;
        public const int Crabson = 1;
        public const int RedSprite = 2;
        public const int AStrangeIdea = 3;
        public const int RockFromAnAlternateUniverse = 4;
        public const int OmegaStaritePainting = 5;
        public const int SpaceSquid = 6;

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            dustType = 7;
            disableSmartCursor = true;
            AddMapEntry(new Color(120, 85, 60), CreateMapEntryName("Trophies"));
            AddMapEntry(new Color(120, 85, 60), CreateMapEntryName("Painting"));
        }

        public override ushort GetMapOption(int i, int j)
        {
            if (Main.tile[i, j].frameX >= 162 && Main.tile[i, j].frameX <= 306
                && Main.tile[i, j].frameY <= 108)
            {
                return 1;
            }
            return 0;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            switch (frameX / 54)
            {
                case 0:
                    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<OmegaStariteTrophy>());
                    break;

                case 1:
                    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<CrabsonTrophy>());
                    break;

                case 2:
                    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<RedSpriteTrophy>());
                    break;

                case AStrangeIdea:
                    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<AStrangeIdea>());
                    break;

                case RockFromAnAlternateUniverse:
                    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<RockFromAnAlternateUniverse>());
                    break;

                case OmegaStaritePainting:
                    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<OmegaStaritePainting>());
                    break;

                case SpaceSquid:
                    Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<SpaceSquidTrophy>());
                    break;
            }
        }
    }
}
