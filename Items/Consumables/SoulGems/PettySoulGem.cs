using Aequus.Items.Misc;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.SoulGems
{
    public class PettySoulGem : SoulGemBase
    {
        public override int Tier => 1;

        public override void SetFilledDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ItemID.Amethyst)
                .AddIngredient<BloodyTearFragment>()
                .AddTile(TileID.Anvils)
                .TryRegisterBefore(ItemID.CursedBullet);
        }
    }
}