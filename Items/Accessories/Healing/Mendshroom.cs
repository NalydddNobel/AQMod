using Aequus.Common.Recipes;
using Aequus.Content;
using Aequus.Items.Materials.Energies;
using Aequus.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Healing
{
    public class Mendshroom : ModItem, ItemHooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;

            SentryAccessoriesDatabase.OnAI.Add(Type, SentryAccessoriesDatabase.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemDefaults.RarityCrabCrevice;
            Item.value = ItemDefaults.ValueCrabCrevice;
            Item.shoot = ModContent.ProjectileType<MendshroomProj>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accMendshroom = Item;
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.Aequus().cMendshroom = dyeItem.dye;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(AequusRecipes.AnyMosshrooms)
                .AddIngredient<AquaticEnergy>()
                .AddTile(TileID.Anvils)
                .TryRegisterBefore(ItemID.CharmofMyths);
        }
    }
}