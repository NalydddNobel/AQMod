using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class SpaceShot : ModItem, IItemOverlaysWorldDraw, IItemOverlaysPlayerDraw
    {
        private readonly BasicOverlay _overlay = new BasicOverlay(AQUtils.GetPath<SpaceShot>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => _overlay;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.damage = 13;
            Item.rare = AQItem.Rarities.StariteWeaponRare;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 15f;
            Item.value = Item.sellPrice(silver: 80);
            Item.useAmmo = AmmoID.Bullet;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item11;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Ranged.SpaceShot>(), damage, knockback, player.whoAmI, type);
            return false;
        }
    }
}