using AQMod.Localization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.NPCs.Town.Robster.QuestTiles
{
    public class JeweledCandelabra : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            AddMapEntry(Robster.JeweledTileMapColor, AQText.ModText("ItemName.JeweledCandelabra"));
            soundStyle = SoundID.Dig;
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Chandeliers };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (HuntSystem.SpecialHuntTileDestroyed(i, j) == false)
                Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<QuestItems.JeweledCandelabra>());
        }
    }
}