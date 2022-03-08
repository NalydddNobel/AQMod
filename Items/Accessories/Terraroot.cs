using AQMod.Items.Materials.Energies;
using AQMod.Items.Recipes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class Terraroot : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.sellPrice(gold: 4);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().ammoRenewal = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<OrganicEnergy>(), 5);
            r.AddIngredient(ItemID.HellstoneBar, 20);
            r.AddRecipeGroup(AQRecipeGroups.ShadowScaleOrTissueSample, 10);
            r.AddIngredient(ItemID.JungleSpores, 8);
            r.AddIngredient(ItemID.Bone, 80);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}