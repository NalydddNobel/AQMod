using AQMod.Localization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.NPCs.Town.Robster.QuestTiles
{
    public class JeweledChalice : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            AddMapEntry(Robster.JeweledTileMapColor, AQText.ModText("ItemName.JeweledChalice"));
            soundStyle = SoundID.Dig;
            disableSmartCursor = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override bool Drop(int i, int j)
        {
            if (HuntSystem.SpecialHuntTileDestroyed(i, j) == false)
                Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<QuestItems.JeweledChalice>());
            return true;
        }
    }
}
