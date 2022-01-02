using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace AQMod.Tiles.Furniture
{
    public sealed class BuriedChests : ContainerType
    {
        protected override string ChestName => "Buried Chest";
        protected override void AddMapEntires()
        {
            AddMapEntry(new Color(151, 107, 75, 255), CreateMapEntryName("BuriedDirtChest"), MapChestName);
        }

        protected override void ChestStatics() // prevent this method from doing anything so this chest doesn't shine, appear on the Metal Detector, or appear when using the Spelunker Potion.
        {
        }

        protected override int ShowHoverItem(Player player, int i, int j, int x, int y)
        {
            return ItemID.Chest;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, chestDrop);
            base.KillMultiTile(i, j, frameX, frameY);
        }
    }
}