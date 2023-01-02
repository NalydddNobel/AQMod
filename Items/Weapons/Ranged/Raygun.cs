using Aequus.Items.Misc.Energies;
using Aequus.Particles.Dusts;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class Raygun : ModItem, ItemHooks.IOnSpawnProjectile
    {
        public static Dictionary<int, Func<Projectile, Color>> BulletColor { get; private set; }

        public override void Load()
        {
            BulletColor = new Dictionary<int, Func<Projectile, Color>>()
            {
                [ProjectileID.Bullet] = (p) => new Color(1, 255, 40, 255),
                [ProjectileID.MeteorShot] = (p) => new Color(30, 255, 200, 255),
                [ProjectileID.CrystalBullet] = (p) => new Color(200, 112, 145, 255),
                [ProjectileID.CrystalShard] = (p) => new Color(200, 112, 145, 255),
                [ProjectileID.CursedBullet] = (p) => new Color(120, 228, 50, 255),
                [ProjectileID.IchorBullet] = (p) => new Color(228, 200, 50, 255),
                [ProjectileID.ChlorophyteBullet] = (p) => new Color(135, 255, 120, 255),
                [ProjectileID.BulletHighVelocity] = (p) => new Color(255, 255, 235, 255),
                [ProjectileID.VenomBullet] = (p) => new Color(128, 30, 255, 255),
                [ProjectileID.NanoBullet] = (p) => new Color(60, 200, 255, 255),
                [ProjectileID.ExplosiveBullet] = (p) => new Color(255, 120, 60, 255),
                [ProjectileID.GoldenBullet] = (p) => new Color(255, 255, 10, 255),
                [ProjectileID.MoonlordBullet] = (p) => new Color(60, 215, 245, 255),
                [ProjectileID.PartyBullet] = (p) => AequusHelpers.GetRainbowColor(p, Main.GlobalTimeWrappedHourly % 6f),
            };
        }

        public override void Unload()
        {
            BulletColor?.Clear();
            BulletColor = null;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.SetWeaponValues(25, 4f, 0);
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 1.5f;
            Item.autoReuse = true;
            Item.UseSound = Aequus.GetSound("Item/raygun");
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.value = ItemDefaults.OmegaStariteValue;
            Item.knockBack = 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4f, -4f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IllegalGunParts)
                .AddIngredient<CosmicEnergy>()
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.PhoenixBlaster);
        }

        public static Color GetColor(Projectile projectile)
        {
            if (BulletColor.TryGetValue(projectile.type, out var color))
            {
                return color(projectile);
            }
            if (Main.netMode == NetmodeID.Server)
            {
                return Color.White;
            }
            var clr = CheckRayColor(projectile);
            BulletColor[projectile.type] = (p) => p.GetAlpha(clr);
            return clr;
        }

        public static Color CheckRayColor(Projectile projectile)
        {
            var texture = TextureAssets.Projectile[projectile.type];
            if (texture == null || !texture.IsLoaded)
            {
                return Color.White;
            }

            int r = 0;
            int g = 0;
            int b = 0;
            int count = 0;

            try
            {
                var clrs = texture.Value.Get1DColorArr(projectile.Frame());

                for (int i = 0; i < clrs.Length; i++)
                {
                    if (clrs[i].A == 255)
                    {
                        r += clrs[i].R;
                        g += clrs[i].G;
                        b += clrs[i].B;
                        count++;
                    }
                }
            }
            catch
            {

            }

            if (count == 0)
                return Color.White;

            return new Color(r / count, g / count, b / count);
        }

        public static void SpawnExplosion(IEntitySource source, Projectile projectile)
        {
            var center = projectile.Center;
            if (Main.netMode != NetmodeID.Server)
            {
                int amt = (int)(75 * (ClientConfig.Instance.HighQuality ? 1f : 0.5f));
                var color = GetColor(projectile).UseA(0);
                for (int i = 0; i < amt; i++)
                {
                    float scale = Main.rand.NextFloat(1f, 3f);
                    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale);
                    var r = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                    d.position = center + r * Main.rand.NextFloat(24f);
                    float speed = Main.rand.NextFloat(9f, 14f);
                    d.velocity = r * (speed - Math.Min(scale * 4f, speed - 0.1f));
                }
            }
            if (projectile.type == ProjectileID.CrystalBullet)
            {
                for (int i = 0; i < 6; i++)
                {
                    var r = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                    var explosionPos = projectile.Center + r * Main.rand.NextFloat(16f, 60f);
                    if (Main.netMode != NetmodeID.Server)
                    {
                        int amt = (int)(35 * (ClientConfig.Instance.HighQuality ? 1f : 0.5f));
                        var color = GetColor(projectile).UseA(0) * 0.8f;
                        for (int j = 0; j < amt; j++)
                        {
                            float scale = Main.rand.NextFloat(1f, 3f);
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale * 0.75f);
                            var r2 = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                            d.position = explosionPos + r2 * Main.rand.NextFloat(6f);
                            float speed = Main.rand.NextFloat(9f, 14f);
                            d.velocity = r2 * (speed - Math.Min(scale * 4f, speed - 0.1f)) * 1.2f;
                        }
                    }
                    if (Main.myPlayer == projectile.owner)
                    {
                        Projectile.NewProjectile(source, explosionPos, Vector2.Normalize(projectile.velocity), ModContent.ProjectileType<RaygunExplosionProj>(), projectile.damage, projectile.knockBack, projectile.owner);
                        Projectile.NewProjectile(source, explosionPos, r * 8f, ProjectileID.CrystalShard, projectile.damage / 10, projectile.knockBack, projectile.owner);
                    }
                }
            }
            else if (projectile.type == ProjectileID.PartyBullet)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f)
                    {
                        var r = f.ToRotationVector2();
                        Projectile.NewProjectile(source, center + r * 50f, r * 8f, ProjectileID.ConfettiGun, 0, 0f, projectile.owner);
                    }
                }
            }
            else if (projectile.type == ProjectileID.ExplosiveBullet)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    int amt = (int)(175 * (ClientConfig.Instance.HighQuality ? 1f : 0.5f));
                    var color = GetColor(projectile).UseA(0) * 1.2f;
                    for (int j = 0; j < amt; j++)
                    {
                        float scale = Main.rand.NextFloat(0.6f, 2.5f);
                        if (Main.rand.NextBool(4))
                        {
                            scale *= 1.5f;
                        }
                        var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale);
                        var r2 = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                        d.position = center + r2 * Main.rand.NextFloat(50f);
                        float speed = Main.rand.NextFloat(9f, 14f);
                        d.velocity = r2 * (speed - Math.Min(scale * 4f, speed - 0.01f)) * 2.15f;
                    }
                }
                if (Main.myPlayer == projectile.owner)
                {
                    for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f)
                    {
                        var r = f.ToRotationVector2();
                        var explosionPos = projectile.Center + r * Main.rand.NextFloat(42f, 68f);
                        Projectile.NewProjectile(source, explosionPos, Vector2.Normalize(projectile.velocity), ModContent.ProjectileType<RaygunExplosionProj>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
            }
            if (Main.myPlayer == projectile.owner)
            {
                // A small bit of velocity is given to this explosion projectile to make it knockback enemies in the correct direction
                // I could just override the modify hit methods and manually apply direction there but blah
                Projectile.NewProjectile(source, projectile.Center, Vector2.Normalize(projectile.velocity), ModContent.ProjectileType<RaygunExplosionProj>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }

        public void OnCreateProjectile(Projectile projectile, AequusProjectile aequusProjectile, IEntitySource source)
        {
            projectile.extraUpdates++;
            projectile.extraUpdates *= 6;
        }
    }
}