using Aequus.Tiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Interactable {
    public class FishSign : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<FishSignTile>());
            Item.value = Item.buyPrice(gold: 2, silver: 50);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;
        }
    }
}