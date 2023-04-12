using Aequus.Content.WorldGeneration;
using Aequus.Items.Materials;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class Hitscanner : ModItem, ItemHooks.IOnSpawnProjectile
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            HardmodeChestBoost.HardmodeJungleChestLoot.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 16f;
            Item.ArmorPenetration = 5;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = Aequus.GetSound("Item/doomShotgun");
            Item.value = Item.sellPrice(gold: 4);
            Item.autoReuse = true;
            Item.knockBack = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, 2f);
        }

        public int GetBulletAmount(int projectileID)
        {
            return projectileID == ProjectileID.ChlorophyteBullet ? 4 : 9;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int amount = GetBulletAmount(type);
            for (int i = 0; i < amount; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)), type, damage, knockback, player.whoAmI);
                if (type == ProjectileID.ChlorophyteBullet)
                {
                    i++;
                }
            }
            return true;
        }

        public void IndirectInheritence(Projectile projectile, AequusProjectile aequusProjectile, IEntitySource source)
        {
            projectile.extraUpdates++;
            if (projectile.type == ProjectileID.ChlorophyteBullet)
            {
                projectile.extraUpdates *= 5;
                projectile.damage *= 2;
            }
            else
            {
                projectile.extraUpdates *= 20;
            }
        }
    }
}