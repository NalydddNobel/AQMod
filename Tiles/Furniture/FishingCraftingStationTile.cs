using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Furniture
{
    public class FishingCraftingStationTile : ModTile
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            name = "FishingCraftingStation";
            return base.Autoload(ref name, ref texture);
        }

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.addTile(Type);
            var name = CreateMapEntryName("FishingCraftingStation");
            AddMapEntry(new Color(222, 200, 200), name);
            soundStyle = SoundID.Dig;
            disableSmartCursor = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<Items.Placeable.CraftingStations.FishingCraftingStation>());
        }
    }
}