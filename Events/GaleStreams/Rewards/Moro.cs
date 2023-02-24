using Aequus.Events.GaleStreams.Misc;
using Aequus.Items;
using Aequus.Items.Misc.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Events.GaleStreams.Rewards
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
            CreateRecipe()
                .AddRecipeGroup(AequusRecipes.AnyFruit, 3)
                .AddIngredient<Fluorescence>(10)
                .AddIngredient<AtmosphericEnergy>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}