using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class JerryClawFlail : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 32;
            item.melee = true;
            item.useTime = 30;
            item.useAnimation = 30;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = AQItem.RarityCrabCrevice;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.JerryClawFlail>();
            item.shootSpeed = 22f;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.Prices.CrabCreviceValue;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 6f;
            item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < 1;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var velocity = new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
            Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI);
            velocity = new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f));
            speedX = velocity.X;
            speedY = velocity.Y;
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
    }
}