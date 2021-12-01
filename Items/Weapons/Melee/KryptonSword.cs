using AQMod.Items.Materials.Energies;
using AQMod.Items.Materials.NobleMushrooms;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class KryptonSword : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 25;
            item.useTime = 20;
            item.useAnimation = 20;
            item.rare = AQItem.Rarities.CrabsonWeaponRare;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(silver: 40);
            item.melee = true;
            item.knockBack = 5f;
            item.scale = 1.1f;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<VineSword>());
            r.AddIngredient(ModContent.ItemType<KryptonMushroom>(), 2);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>(), 3);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}