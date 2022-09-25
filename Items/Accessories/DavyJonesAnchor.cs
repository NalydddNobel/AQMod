using Aequus.Items.Misc.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class DavyJonesAnchor : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accDavyJonesAnchor = Item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("IronBar", 8)
                .AddIngredient(ItemID.Rope, 50)
                .AddIngredient<AquaticEnergy>()
                .AddTile(ItemID.IronAnvil)
                .Register();
        }
    }
}