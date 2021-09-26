using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable
{
    public class FishingCraftingStation : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<Tiles.FishingCraftingStation>();
            item.value = Item.buyPrice(gold: 5);
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }
    }
}