using Aequus.Items.Materials.Fish;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Sentry {
    public class SentryPotion : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { new Color(208, 101, 32, 0), new Color(241, 216, 109, 0), new Color(138, 76, 31, 0), };
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.SummoningPotion);
            Item.buffType = ModContent.BuffType<SentryBuff>();
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<IcebergFish>()
                .AddIngredient(ItemID.Shiverthorn)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}