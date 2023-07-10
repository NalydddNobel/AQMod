using Aequus.Common.Items;
using Aequus.Items.Materials.Energies;
using Aequus.Tiles.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Swords.ThrashBag {
    public class ThrashBag : ModItem {
        public override void SetDefaults() {
            Item.SetWeaponValues(25, 4.5f, 6);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemDefaults.RarityCrabCrevice;
            Item.autoReuse = true;
            Item.value = ItemDefaults.ValueCrabCrevice;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.TrashCan)
                .AddIngredient<AquaticEnergy>()
                .AddTile<RecyclingMachineTile>()
                .Register();
        }
    }
}