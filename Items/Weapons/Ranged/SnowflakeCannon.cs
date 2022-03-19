using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Recipes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class SnowflakeCannon : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 16;
            item.useAnimation = 16;
            item.autoReuse = true;
            item.damage = 13;
            item.rare = AQItem.RarityGaleStreams;
            item.ranged = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shoot = ProjectileID.SnowBallFriendly;
            item.shootSpeed = 18f;
            item.value = AQItem.Prices.GaleStreamsWeaponValue;
            item.useAmmo = AmmoID.Snowball;
            item.knockBack = 5.6f;
            item.UseSound = SoundID.Item11;
            item.noMelee = true;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() < 0.66f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            Main.projectile[p].scale = 1.8f;
            Main.projectile[p].alpha = 10;
            p = Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(0.1f + Main.rand.NextFloat(-0.02f, 0.02f)), type, damage, knockBack, player.whoAmI);
            Main.projectile[p].scale = 1.4f;
            Main.projectile[p].alpha = 20;
            p = Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(-0.1f + Main.rand.NextFloat(-0.02f, 0.02f)), type, damage, knockBack, player.whoAmI);
            Main.projectile[p].scale = 1.4f;
            Main.projectile[p].alpha = 20;
            p = Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(0.2f + Main.rand.NextFloat(-0.02f, 0.02f)), type, damage, knockBack, player.whoAmI);
            Main.projectile[p].scale = 1f;
            Main.projectile[p].alpha = 60;
            p = Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(-0.2f + Main.rand.NextFloat(-0.02f, 0.02f)), type, damage, knockBack, player.whoAmI);
            Main.projectile[p].scale = 1f;
            Main.projectile[p].alpha = 60;
            return false;
        }

        public override void AddRecipes()
        {
            AQRecipes.GaleStreamsMinibossWeapon(mod, ItemID.SnowballCannon, ModContent.ItemType<SiphonTentacle>(), this);
        }
    }
}