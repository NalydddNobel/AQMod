using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable.Nature;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee.Spear
{
    public class ArgonSpear : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.melee = true;
            item.damage = 15;
            item.knockBack = 1f;
            item.useAnimation = 32;
            item.useTime = 48;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.rare = AQItem.Rarities.CrabCreviceRare;
            item.value = Item.sellPrice(silver: 40);
            item.shootSpeed = 7.5f;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.ArgonSpear>();
            item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Spear);
            r.AddIngredient(ModContent.ItemType<ArgonMushroom>(), 2);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>(), 3);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}