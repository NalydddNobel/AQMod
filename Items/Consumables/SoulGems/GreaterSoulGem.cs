using Aequus.Items.Misc;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.SoulGems
{
    public class GreaterSoulGem : SoulGemBase
    {
        public override int Tier => 3;

        public override void SetFilledDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient(ItemID.Sapphire)
                .AddIngredient(ItemID.SoulofNight, 1)
                .AddIngredient<BloodyTearFragment>()
                .AddTile(TileID.Anvils)
                .TryRegisterBefore(ItemID.CursedBullet);
        }
    }
}