using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class StarPhish : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 8;
            item.ranged = true;
            item.damage = 18;
            item.useTime = 31;
            item.useAnimation = 31;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 2f;
            item.autoReuse = true;
            item.value = Item.sellPrice(silver: 10);
            item.useAmmo = AmmoID.Dart;
            item.shoot = ProjectileID.Seed;
            item.shootSpeed = 25f;
            item.UseSound = SoundID.Item65;
            item.noMelee = true;
            item.rare = AQItem.Rarities.CrabsonWeaponRare;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int dustAmount = Main.rand.Next(35, 50);
            var normalizedVelocity = Vector2.Normalize(new Vector2(speedX, speedY));
            Vector2 spawnPosition = position + normalizedVelocity * 30f + new Vector2(0, -4f);
            for (int i = 0; i < dustAmount; i++)
            {
                int d = Dust.NewDust(spawnPosition, 10, 10, 33);
                Vector2 velocity = normalizedVelocity.RotatedBy(Main.rand.NextFloat(-0.314f, 0.314f));
                Main.dust[d].velocity.X = velocity.X * Main.rand.NextFloat(6f, 12f);
                Main.dust[d].velocity.Y = velocity.Y * Main.rand.NextFloat(6f, 12f);
            }
            dustAmount = Main.rand.Next(8, 12);
            for (int i = 0; i < dustAmount; i++)
            {
                int d = Dust.NewDust(spawnPosition, 10, 10, 15);
                Vector2 velocity = normalizedVelocity.RotatedBy(Main.rand.NextFloat(-0.157f, 0.157f));
                Main.dust[d].velocity.X = velocity.X * Main.rand.NextFloat(3f, 6f);
                Main.dust[d].velocity.Y = velocity.Y * Main.rand.NextFloat(3f, 6f);
            }
            return true;
        }
    }
}