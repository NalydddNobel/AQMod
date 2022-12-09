using Aequus.Items.Misc.Festive;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Vanity
{
    public class EyeGlint : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 20);
            Item.vanity = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Aequus().eyeGlint = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.Aequus().eyeGlint = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Goggles)
                .AddIngredient(ItemID.SnowBlock, 150)
                .AddIngredient<XmasEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}