using Aequus.Buffs;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions
{
    public class NecromancyPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { Color.HotPink.UseA(50), };
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SummoningPotion);
            Item.buffType = ModContent.BuffType<NecromancyPotionBuff>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Flounder)
                .AddIngredient(ItemID.Waterleaf)
                .AddTile(TileID.Bottles)
                .TryRegisterAfter(ItemID.SummoningPotion);
        }
    }
}