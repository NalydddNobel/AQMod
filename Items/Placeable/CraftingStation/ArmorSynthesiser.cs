using Aequus.Tiles.CraftingStation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.CraftingStation
{
    public class ArmorSynthesizer : ModItem
    {
        public override string Texture => Aequus.PlaceholderItem;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ArmorSynthesizerTile>());
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Blue;
        }
    }
}