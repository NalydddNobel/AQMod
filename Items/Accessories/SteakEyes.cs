using Aequus.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class SteakEyes : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(24, 24);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.bloodDiceDamage = Math.Max(aequus.bloodDiceDamage, 0.25f) + 0.25f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HighSteaks>()
                .AddIngredient<BloodyTearFragment>(12)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}