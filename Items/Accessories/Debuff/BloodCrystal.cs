using Aequus.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Debuff
{
    public class BloodCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            if (aequus.debuffLifeSteal > 0)
            {
                aequus.debuffLifeSteal = Math.Max(aequus.debuffLifeSteal / 2, 1);
            }
            else
            {
                player.Aequus().debuffLifeSteal = 20;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BloodyTearstone>(8)
                .AddIngredient(ItemID.DemoniteBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<BloodyTearstone>(8)
                .AddIngredient(ItemID.CrimtaneBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}