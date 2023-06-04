using Aequus.Common.Preferences;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Weapons.Ranged;
using Aequus.Particles.Dusts;
using Aequus.Projectiles;
using Aequus.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged {
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
                [ProjectileID.PartyBullet] = (p) => Helper.GetRainbowColor(p, Main.GlobalTimeWrappedHourly % 6f),
            };
        }

        public override void Unload()
        {
            BulletColor?.Clear();
            BulletColor = null;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.SetWeaponValues(32, 4f, 0);
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 1.5f;
            Item.autoReuse = true;
            Item.UseSound = AequusSounds.raygun with { Volume = 0.5f };
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.value = ItemDefaults.ValueOmegaStarite;
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
                .AddIngredient<StariteMaterial>(12)
                .AddIngredient<CosmicEnergy>()
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
            if (source is EntitySource_OnHit onHit && onHit.Victim is NPC npc)
            {
                npc.SetIDStaticHitCooldown<RaygunExplosionProj>(10);
            }
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
                        Projectile.NewProjectile(source, explosionPos, Vector2.Normalize(projectile.velocity), ModContent.ProjectileType<RaygunExplosionProj>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
                    }
                }
            }
            if (Main.myPlayer == projectile.owner)
            {
                // A small bit of velocity is given to this explosion projectile to make it knockback enemies in the correct direction
                // I could just override the modify hit methods and manually apply direction there but blah
                Projectile.NewProjectile(source, projectile.Center, Vector2.Normalize(projectile.velocity), ModContent.ProjectileType<RaygunExplosionProj>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
            }
        }

        public void InitalizeProjectile(Projectile projectile, AequusProjectile aequusProjectile) {
            projectile.extraUpdates++;
            projectile.extraUpdates *= 6;
        }
    }
}

namespace Aequus.Projectiles {
    public partial class AequusProjectile : GlobalProjectile
    {
        public void AI_Raygun(Projectile projectile)
        {
            if (sourceItemUsed == ModContent.ItemType<Raygun>())
            {
                if (Main.myPlayer == projectile.owner && projectile.numUpdates == -1 && projectile.velocity.Length() > 1f)
                {
                    int p = Projectile.NewProjectile(new EntitySource_Parent(projectile), projectile.Center, Vector2.Normalize(projectile.velocity) * 0.01f,
                        ModContent.ProjectileType<RaygunTrailProj>(), 0, 0f, projectile.owner);
                    Main.projectile[p].rotation = projectile.rotation;
                    Main.projectile[p].netUpdate = true;
                    Main.projectile[p].ModProjectile<RaygunTrailProj>().color = Raygun.GetColor(projectile).UseA(0);
                }
                if (projectile.type == ProjectileID.ChlorophyteBullet)
                {
                    projectile.alpha = 255;
                }
            }
        }

        public bool PreDraw_Raygun(Projectile projectile)
        {
            if (sourceItemUsed == ModContent.ItemType<Raygun>())
            {
                if (!Raygun.BulletColor.ContainsKey(projectile.type))
                {
                    var clr = Raygun.CheckRayColor(projectile);
                    Raygun.BulletColor[projectile.type] = (p) => p.GetAlpha(clr);
                }
                return projectile.velocity.Length() < 1f;
            }
            return true;
        }

        public void OnHit_Raygun(Projectile projectile, Entity victim)
        {
            if (sourceItemUsed == ModContent.ItemType<Raygun>())
            {
                Raygun.SpawnExplosion(projectile.GetSource_OnHit(victim), projectile);
            }
        }

        public void Kill_Raygun(Projectile projectile)
        {
            if (sourceItemUsed == ModContent.ItemType<Raygun>())
            {
                Raygun.SpawnExplosion(projectile.GetSource_Death(), projectile);
            }
        }
    }
}

namespace Aequus.Projectiles.Ranged {
    public class RaygunExplosionProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 4;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = Projectile.timeLeft + 2;
            Projectile.penetrate = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.75f);
        }
    }

    public class RaygunTrailProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PrincessWeapon;

        public Color color;
        public static Dictionary<int, Action<Projectile, Projectile>> OnSpawnEffects { get; private set; }

        public override void Load()
        {
            OnSpawnEffects = new Dictionary<int, Action<Projectile, Projectile>>()
            {
                [ProjectileID.CrystalShard] = (p, parent) => p.scale *= 0.4f,
            };
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 24;
            color = Color.White;
            color.A = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Helper.HereditarySource(source, out var entity))
            {
                if (entity is Projectile parentProjectile)
                {
                    if (OnSpawnEffects.TryGetValue(parentProjectile.type, out var action))
                    {
                        action(Projectile, parentProjectile);
                    }
                }
            }
        }

        public override void AI()
        {
            Projectile.scale += 0.0175f;
            Projectile.alpha += 15;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = 1f - Projectile.alpha / 255f;
            var texture = TextureAssets.Projectile[Type];
            var origin = texture.Size() / 2f;
            var scale = new Vector2(Projectile.scale * 0.11f, Projectile.scale * 0.245f);
            Main.spriteBatch.Draw(texture.Value, Projectile.Center - Main.screenPosition, null, color * opacity, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
            writer.Write(Projectile.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            color.R = reader.ReadByte();
            color.G = reader.ReadByte();
            color.B = reader.ReadByte();
            color.A = 0;
            Projectile.scale = reader.ReadSingle();
        }
    }
}