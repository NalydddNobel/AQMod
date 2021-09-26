using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public class Bottles : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(200, 200, 200), Lang.GetItemName(ItemID.Bottle));
            soundStyle = SoundID.Dig;
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Bottles };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override bool Drop(int i, int j)
        {
            Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<Items.Misc.QuestRobster.JeweledChalice>());
            return true;
        }
    }
}
