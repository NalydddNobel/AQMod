using Aequus.Items.Misc;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.SoulGems
{
    public class GrandSoulGem : SoulGemBase
    {
        public override int Tier => 4;

        protected override void SetFilledDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.Emerald)
                .AddIngredient(ItemID.Ectoplasm, 2)
                .AddIngredient<BloodyTearFragment>()
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterBefore(ItemID.CursedBullet);
        }
    }
}