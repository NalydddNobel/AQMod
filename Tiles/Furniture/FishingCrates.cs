using Aequus.Items.Consumables.LootBags;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture
{
    public class FishingCrates : ModTile
    {
        public const int CrabCreviceCrate = 0;
        public const int CrabCreviceCrateHard = 1;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);

            AddMapEntry(Color.Brown * 1.5f, CreateMapEntryName());
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, GetItemDrop(frameX));
        }

        public static int GetItemDrop(int frameX)
        {
            switch (frameX / 36)
            {
                case 1:
                    return ModContent.ItemType<CrabCreviceCrateHard>();
                default:
                    return ModContent.ItemType<CrabCreviceCrate>();
            }
        }
    }
}