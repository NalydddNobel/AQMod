using Aequus.Tiles.Ambience;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Pollen
{
    public class MistralSeeds : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MistralTile>());
            Item.value = Item.sellPrice(silver: 2);
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;
        }
    }
}