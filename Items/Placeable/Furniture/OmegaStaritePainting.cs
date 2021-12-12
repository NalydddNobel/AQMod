using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Furniture
{
    public class OmegaStaritePainting : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.value = Item.buyPrice(gold: 10);
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<Tiles.Furniture.Trophies>();
            item.placeStyle = Tiles.Furniture.Trophies.OmegaStaritePainting;
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }
    }
}