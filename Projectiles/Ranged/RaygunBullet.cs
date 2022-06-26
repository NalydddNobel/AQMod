using Aequus.Particles.Dusts;
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

namespace Aequus.Projectiles.Ranged
{
    public class RaygunBullet : ModProjectile
    {
        public static Dictionary<int, Color> RaygunColors { get; private set; }

        public override string Texture => Aequus.BlankTexture;

        public int projType;
        public int trailTimer;

        public override void SetStaticDefaults()
        {
            RaygunColors = new Dictionary<int, Color>()
            {
                [ProjectileID.Bullet] = new Color(1, 255, 40, 255),
                [ProjectileID.MeteorShot] = new Color(30, 255, 200, 255),
                [ProjectileID.CrystalBullet] = new Color(200, 112, 145, 255),
                [ProjectileID.CursedBullet] = new Color(120, 228, 50, 255),
                [ProjectileID.IchorBullet] = new Color(228, 200, 50, 255),
                [ProjectileID.ChlorophyteBullet] = new Color(135, 255, 120, 255),
                [ProjectileID.BulletHighVelocity] = new Color(255, 255, 235, 255),
                [ProjectileID.VenomBullet] = new Color(128, 30, 255, 255),
                [ProjectileID.NanoBullet] = new Color(60, 200, 255, 255),
                [ProjectileID.ExplosiveBullet] = new Color(255, 120, 60, 255),
                [ProjectileID.GoldenBullet] = new Color(255, 255, 10, 255),
                [ProjectileID.MoonlordBullet] = new Color(60, 215, 245, 255),
            };
        }

        public override void Unload()
        {
            RaygunColors?.Clear();
            RaygunColors = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, 180);
            Projectile.friendly = true;
            Projectile.hide = true;
            Projectile.aiStyle = 1;
            trailTimer = 7;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo itemUseWithAmmo)
            {
                projType = ContentSamples.ItemsByType[itemUseWithAmmo.AmmoItemIdUsed].shoot;
            }
            else
            {
                projType = ProjectileID.Bullet;
            }
            StealSomeStats(GetProjectileInstance());
        }
        private void StealSomeStats(Projectile projToClone)
        {
            if (projToClone != null)
            {
                //Projectile.DamageType = baseProj.DamageType;
                //Projectile.aiStyle = baseProj.aiStyle;
                Projectile.penetrate = projToClone.penetrate;
                Projectile.usesLocalNPCImmunity = projToClone.usesLocalNPCImmunity;
                Projectile.localNPCHitCooldown = projToClone.localNPCHitCooldown;
                Projectile.usesIDStaticNPCImmunity = projToClone.usesIDStaticNPCImmunity;
                Projectile.idStaticNPCHitCooldown = projToClone.idStaticNPCHitCooldown;
                Projectile.scale = projToClone.scale;
                Projectile.ignoreWater = projToClone.ignoreWater;
                Projectile.tileCollide = projToClone.tileCollide;
                Projectile.extraUpdates = projToClone.extraUpdates;

                AIType = projToClone.type;
            }
            else
            {
                Projectile.aiStyle = -1;
                Projectile.DamageType = DamageClass.Ranged;
            }
            SetDefaults();
            Projectile.extraUpdates++;
            Projectile.extraUpdates *= 6;
            //Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (AIType == ProjectileID.ChlorophyteBullet)
            {
                Projectile.alpha = 255;
            }
            if (trailTimer <= 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    int p = Projectile.NewProjectile(new EntitySource_Parent(Projectile), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 0.01f, ModContent.ProjectileType<RayTrailEffect>(), 0, 0f, Projectile.owner);
                    Main.projectile[p].rotation = Projectile.velocity.ToRotation();
                    ((RayTrailEffect)Main.projectile[p].ModProjectile).color = GetColor().UseA(0);
                    Projectile.netUpdate = true;
                }
                trailTimer = 6;
            }
            trailTimer--;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 4;
            height = 4;
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projType > ProjectileID.None)
            {
                var value = GetProjectileInstance().ModProjectile?.OnTileCollide(oldVelocity);
                if (projType == ProjectileID.MeteorShot || projType == ProjectileID.NanoBullet)
                {
                    if (Projectile.velocity.X != oldVelocity.X)
                    {
                        Projectile.velocity.X = -oldVelocity.X;
                    }
                    if (Projectile.velocity.Y != oldVelocity.Y)
                    {
                        Projectile.velocity.Y = -oldVelocity.Y;
                    }
                    if (projType == ProjectileID.MeteorShot)
                    {
                        Projectile.penetrate--;
                        if (Projectile.penetrate == 0)
                        {
                            Projectile.Kill();
                        }
                    }
                    else
                    {
                        Projectile.ai[1]++;
                        if ((int)Projectile.ai[1] == 1)
                        {
                            Projectile.damage = (int)(Projectile.damage * 0.66f);
                        }
                        if (Projectile.ai[1] >= 2f)
                        {
                            Projectile.Kill();
                        }
                        int target = Projectile.FindTargetWithLineOfSight();
                        if (target != -1)
                        {
                            NPC nPC2 = Main.npc[target];
                            Projectile.Distance(nPC2.Center);
                            Projectile.velocity = Projectile.DirectionTo(nPC2.Center).SafeNormalize(-Vector2.UnitY) * Projectile.velocity.Length();
                            Projectile.netUpdate = true;
                        }
                    }
                    return value.GetValueOrDefault(false);
                }
                return value.GetValueOrDefault(true);
            }
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            GetProjectileInstance()?.ModProjectile?.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projType > ProjectileID.None)
            {
                if (projType == ProjectileID.CursedBullet)
                {
                    target.AddBuff(BuffID.CursedInferno, 240);
                }
                else if (projType == ProjectileID.IchorBullet)
                {
                    target.AddBuff(BuffID.Ichor, 240);
                }
                else if (projType == ProjectileID.VenomBullet)
                {
                    target.AddBuff(BuffID.Venom, 240);
                }
                else if (projType == ProjectileID.NanoBullet)
                {
                    target.AddBuff(BuffID.Confused, 1200);
                }
                else if (projType == ProjectileID.GoldenBullet)
                {
                    target.AddBuff(BuffID.Midas, 1200);
                }
                if (Projectile.penetrate != 1)
                {
                    SpawnExplosion();
                }
                GetProjectileInstance().ModProjectile?.OnHitNPC(target, damage, knockback, crit);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(trailTimer);
            writer.Write(projType);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            trailTimer = reader.ReadInt32();
            projType = reader.ReadInt32();
        }

        public override void Kill(int timeLeft)
        {
            SpawnExplosion();
        }

        public virtual Color GetColor()
        {
            if (projType == ProjectileID.PartyBullet)
            {
                return AequusHelpers.GetRainbowHue(Projectile, Main.GlobalTimeWrappedHourly % 6f);
            }
            if (RaygunColors.TryGetValue(projType, out var color))
            {
                return color;
            }
            return new Color(255, 255, 255, 255);
        }

        private void SpawnExplosion()
        {
            var center = Projectile.Center;
            if (Main.netMode != NetmodeID.Server)
            {
                int amt = (int)(75 * (ClientConfig.Instance.HighQuality ? 1f : 0.5f));
                var color = GetColor().UseA(0);
                for (int i = 0; i < amt; i++)
                {
                    float scale = Main.rand.NextFloat(1f, 3f);
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale);
                    var r = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                    d.position = center + r * Main.rand.NextFloat(24f);
                    float speed = Main.rand.NextFloat(9f, 14f);
                    d.velocity = r * (speed - Math.Min(scale * 4f, speed - 0.1f));
                }
            }
            if (projType == ProjectileID.CrystalBullet)
            {
                for (int i = 0; i < 6; i++)
                {
                    var r = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                    var explosionPos = Projectile.Center + r * Main.rand.NextFloat(16f, 60f);
                    if (Main.netMode != NetmodeID.Server)
                    {
                        int amt = (int)(35 * (ClientConfig.Instance.HighQuality ? 1f : 0.5f));
                        var color = GetColor().UseA(0) * 0.8f;
                        for (int j = 0; j < amt; j++)
                        {
                            float scale = Main.rand.NextFloat(1f, 3f);
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale * 0.75f);
                            var r2 = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                            d.position = explosionPos + r2 * Main.rand.NextFloat(6f);
                            float speed = Main.rand.NextFloat(9f, 14f);
                            d.velocity = r2 * (speed - Math.Min(scale * 4f, speed - 0.1f)) * 1.2f;
                        }
                    }
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(new EntitySource_Parent(Projectile), explosionPos, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<RayExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Projectile.NewProjectile(new EntitySource_Parent(Projectile), explosionPos, r * 8f, ProjectileID.CrystalShard, Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
            else if (projType == ProjectileID.PartyBullet)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f)
                    {
                        var r = f.ToRotationVector2();
                        Projectile.NewProjectile(new EntitySource_Parent(Projectile), center + r * 50f, r * 8f, ProjectileID.ConfettiGun, 0, 0f, Projectile.owner);
                    }
                }
            }
            else if (projType == ProjectileID.ExplosiveBullet)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    int amt = (int)(175 * (ClientConfig.Instance.HighQuality ? 1f : 0.5f));
                    var color = GetColor().UseA(0) * 1.2f;
                    for (int j = 0; j < amt; j++)
                    {
                        float scale = Main.rand.NextFloat(0.6f, 2.5f);
                        if (Main.rand.NextBool(4))
                        {
                            scale *= 1.5f;
                        }
                        var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale);
                        var r2 = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                        d.position = center + r2 * Main.rand.NextFloat(50f);
                        float speed = Main.rand.NextFloat(9f, 14f);
                        d.velocity = r2 * (speed - Math.Min(scale * 4f, speed - 0.01f)) * 2.15f;
                    }
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f)
                    {
                        var r = f.ToRotationVector2();
                        var explosionPos = Projectile.Center + r * Main.rand.NextFloat(42f, 68f);
                        Projectile.NewProjectile(new EntitySource_Parent(Projectile), explosionPos, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<RayExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
            if (Main.myPlayer == Projectile.owner)
            {
                // A small bit of velocity is given to this explosion projectile to make it knockback enemies in the correct direction
                // I could just override the modify hit methods and manually apply direction there but blah
                Projectile.NewProjectile(new EntitySource_Parent(Projectile), Projectile.Center, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<RayExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public virtual Projectile GetProjectileInstance()
        {
            var proj = new Projectile();
            proj.SetDefaults(projType);
            return proj;
        }
    }

    public sealed class RayExplosion : ModProjectile
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
    }

    public sealed class RayTrailEffect : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PrincessWeapon;

        public Color color;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 24;
            color = Color.White;
            color.A = 0;
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
            writer.Write(color.A);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            color.R = reader.ReadByte();
            color.G = reader.ReadByte();
            color.B = reader.ReadByte();
            color.A = reader.ReadByte();
        }
    }
}