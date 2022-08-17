using Aequus.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public sealed class Hitscanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
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
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = Aequus.GetSound("doomshotgun");
            Item.value = Item.sellPrice(gold: 7, silver: 50);
            Item.autoReuse = true;
            Item.knockBack = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, 2f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 10; i++)
            {
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)), type, damage, knockback, player.whoAmI);
                Main.projectile[p].extraUpdates++;
                if (type == ProjectileID.ChlorophyteBullet)
                {
                    Main.projectile[p].extraUpdates *= 15;
                    Main.projectile[p].damage *= 2;
                    i++;
                }
                else
                {
                    Main.projectile[p].extraUpdates *= 50;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.CobaltBar, 10)
                .AddIngredient<DemonicEnergy>(3)
                .AddTile(TileID.Anvils)
                .RegisterAfter(ItemID.OnyxBlaster);

            CreateRecipe()
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.PalladiumBar, 10)
                .AddIngredient<DemonicEnergy>(3)
                .AddTile(TileID.Anvils)
                .RegisterAfter(ItemID.OnyxBlaster);
        }
    }
}