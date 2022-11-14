using Aequus.Items.Misc;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.SoulGems
{
    public class LesserSoulGem : SoulGemBase
    {
        public override int Tier => 2;

        public override void SetFilledDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ItemID.Topaz)
                .AddIngredient(ItemID.Bone, 10)
                .AddIngredient<BloodyTearFragment>()
                .AddTile(TileID.Anvils)
                .TryRegisterBefore(ItemID.CursedBullet);
        }
    }
}