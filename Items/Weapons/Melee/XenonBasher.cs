using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable.Nature;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class XenonBasher : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 32;
            item.useTime = 32;
            item.useAnimation = 32;
            item.rare = AQItem.Rarities.CrabCreviceRare;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(silver: 50);
            item.melee = true;
            item.knockBack = 4f;
            item.scale = 1.2f;
            item.channel = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.XenonBasherHammer>();
            item.noUseGraphic = true;
            item.noMelee = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 250);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<VineSword>());
            r.AddIngredient(ModContent.ItemType<XenonMushroom>(), 2);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}