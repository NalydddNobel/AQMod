using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable.Nature;
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
            item.damage = 18;
            item.useTime = 20;
            item.useAnimation = 20;
            item.rare = AQItem.RarityCrabCrevice;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(silver: 40);
            item.melee = true;
            item.knockBack = 5f;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.KryptonBoomerang>();
            item.shootSpeed = 5f;
            item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.WoodenBoomerang);
            r.AddIngredient(ModContent.ItemType<KryptonMushroom>(), 2);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}