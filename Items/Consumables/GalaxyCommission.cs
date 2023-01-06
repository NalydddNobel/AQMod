using Aequus.Items.Misc.Energies;
using Aequus.Items.Tools.Camera;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class GalaxyCommission : ModItem
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
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override bool? UseItem(Player player)
        {
            player.Aequus().usedPermaBuildBuffRange = true;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShutterstockerClipAmmo>()
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.Anvils)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .Register();
        }
    }
}