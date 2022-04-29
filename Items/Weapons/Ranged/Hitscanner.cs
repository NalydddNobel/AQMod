using Aequus.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public sealed class Hitscanner : ModItem
    {
        public override bool OnlyShootOnSwing => true;

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/doomshotgun");
            Item.value = Item.sellPrice(gold: 7, silver: 50);
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 1f;
            Item.useTime = 10;
            Item.useAnimation = 60;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(6f, 6f);
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
                .AddIngredient(ItemID.MythrilBar, 12)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddIngredient<DemonicEnergy>(5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}