using Aequus.Buffs;
using Aequus.Content.Fishing.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions {
    public class ManathirstPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { new Color(83, 0, 234, 0), new Color(80, 0, 162, 0), };
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ManaRegenerationPotion);
            Item.buffType = ModContent.BuffType<ManathirstBuff>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Depthscale>()
                .AddIngredient(ItemID.Deathweed)
                .AddTile(TileID.Bottles)
                .TryRegisterBefore(ItemID.ManaRegenerationPotion);
        }
    }
}