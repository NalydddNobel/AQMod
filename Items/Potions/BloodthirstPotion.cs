using Aequus.Buffs;
using Aequus.Items.Materials.Fish;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions {
    public class BloodthirstPotion : ModItem {
        public override void SetStaticDefaults() {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { new Color(234, 0, 83, 0), new Color(162, 0, 80, 0), };
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.HeartreachPotion);
            Item.buffType = ModContent.BuffType<BloodthirstBuff>();
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Leecheel>()
                .AddIngredient(ItemID.Deathweed)
                .AddTile(TileID.Bottles)
                .TryRegisterBefore(ItemID.HeartreachPotion);
        }
    }
}