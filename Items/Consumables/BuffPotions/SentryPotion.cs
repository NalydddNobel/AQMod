using Aequus.Buffs;
using Aequus.Items.Misc.Fish;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.BuffPotions
{
    public class SentryPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { new Color(208, 101, 32, 0), new Color(241, 216, 109, 0), new Color(138, 76, 31, 0), };
            SacrificeTotal = 20;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SummoningPotion);
            Item.buffType = ModContent.BuffType<SentryBuff>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<IcebergFish>()
                .AddIngredient(ItemID.Shiverthorn)
                .AddTile(TileID.Bottles)
                .Register((r) => r.SortAfterFirstRecipesOf(ItemID.SummoningPotion));
        }
    }
}