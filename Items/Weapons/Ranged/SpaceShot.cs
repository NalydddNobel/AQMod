using AQMod.Assets.LegacyItemOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class SpaceShot : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyGlowmaskOverlay(this.GetPath("_Glow"), new Color(200, 200, 200, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 30;
            item.useAnimation = 30;
            item.damage = 13;
            item.rare = AQItem.Rarities.StariteWeaponRare;
            item.ranged = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 15f;
            item.value = Item.sellPrice(silver: 80);
            item.useAmmo = AmmoID.Bullet;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item11;
            item.noMelee = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<Projectiles.Ranged.SpaceShot>(), damage, knockBack, player.whoAmI, type);
            return false;
        }
    }
}