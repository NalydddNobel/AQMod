using Aequus.Items.Misc.Energies;
using Aequus.Items.Misc.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Permanent
{
    public class Moro : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item92;
            Item.value = Item.sellPrice(gold: 2);
            Item.maxStack = 9999;
        }

        public override bool? UseItem(Player player)
        {
            if (!player.Aequus().moroSummonerFruit)
            {
                player.Aequus().moroSummonerFruit = true;
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            foreach (var i in AequusItem.FruitIDs)
            {
                CreateRecipe()
                    .AddIngredient(i, 3)
                    .AddIngredient<Fluorescence>(10)
                    .AddIngredient<AtmosphericEnergy>()
                    .AddTile(TileID.Anvils)
                    .Register();
            }
        }
    }
}