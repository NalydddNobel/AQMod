using AQMod.Assets;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class PowPunch : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 24;
            item.melee = true;
            item.useTime = 30;
            item.useAnimation = 30;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Green;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.PowPunch>();
            item.shootSpeed = 16f;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.DemonSiegeWeaponValue;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 8f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] <= 0;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 1f);
            Main.projectile[p].Center = Projectiles.Melee.PowPunch.PowRestingPosition(Main.projectile[p], player);
            p = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, -1f);
            Main.projectile[p].Center = Projectiles.Melee.PowPunch.PowRestingPosition(Main.projectile[p], player);
            return false;
        }
    }
}