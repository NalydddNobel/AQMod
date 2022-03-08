using AQMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged.Ammo
{
    public class DoubleShot : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 4;
            item.consumable = true;
            item.maxStack = 999;
            item.ranged = true;
            item.width = 4;
            item.height = 4;
            item.rare = AQItem.RarityCrabCrevice;
            item.shoot = ModContent.ProjectileType<DoubleShotProj>();
            item.shootSpeed = 1.5f;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.Prices.GaleStreamsWeaponValue;
            item.noMelee = true;
            item.knockBack = 1f;
            item.ammo = AmmoID.Bullet;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextBool();
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.MusketBall, 70);
            r.AddIngredient(ModContent.ItemType<Materials.CrustaciumBar>());
            r.AddTile(TileID.Anvils);
            r.SetResult(this, 70);
            r.AddRecipe();
        }
    }
}