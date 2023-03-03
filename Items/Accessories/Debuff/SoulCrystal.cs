using Aequus.Items.Materials.Gems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Debuff
{
    public class SoulCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().debuffDamage += 40; // Yea, this is weird, trust me. But dealing x4 the damage keeps the damage numbers spawning at a similar rate for some reason...?
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulGemFilled>(5)
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}